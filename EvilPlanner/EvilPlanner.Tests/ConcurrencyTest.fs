namespace EvilPlanner.Tests

open System.IO
open System.Reflection
open System.Threading.Tasks

open Xunit

open EvilPlanner.Core
open ForneverMind.TestFramework.StorageUtils

type ConcurrencyTest() =
    let config =
        let directory = AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
        { databasePath = Path.Combine(directory, "testDb.dat") }

    let clock: IClock = SystemClock() // TODO[#163]: Replace with static clock

    [<Fact>]
    member __.ConcurrencyTest() : Task<unit> =
        do
            reinitializeDatabase config
            use database = Storage.openDatabase config
            database.ReadWriteTransaction clearDailyQuotes

        let concurrencyLevel = 20
        async {
            use database = Storage.openDatabase config
            let tasks =
                seq { 1 .. concurrencyLevel }
                |> Seq.map (fun _ -> async { return executeTransaction clock database })
            let! quotes = Async.Parallel tasks

            let count = database.ReadOnlyTransaction countDailyQuotes
            Assert.Equal (1, count)

            let successCount =
                quotes
                |> Seq.filter Option.isSome
                |> Seq.length
            Assert.Equal (concurrencyLevel, successCount)
        } |> Async.StartAsTask
