namespace EvilPlanner.Tests

open System
open System.IO
open System.Reflection
open System.Threading.Tasks

open LiteDB
open Xunit

open EvilPlanner.Core
open EvilPlanner.Logic.QuoteLogic

type ConcurrencyTest() =
    let config =
        let directory = AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
        { databasePath = Path.Combine(directory, "testDb.dat") }

    let clock = SystemClock() // TODO: Replace with static clock

    let dailyQuotes(db : LiteDatabase) = db.GetCollection<DailyQuote>("dailyQuotes")
    let getDailyQuotes(db : LiteDatabase) = (dailyQuotes db).FindAll()

    let reinitializeDatabase() =
        if File.Exists config.databasePath
        then File.Delete config.databasePath

        Migrations.migrateDatabase config

    let clearDailyQuotes(db : LiteDatabase) =
        ignore <| (dailyQuotes db).Delete(Query.All())

    let executeTransaction database =
        let today = DateTime.UtcNow.Date
        getQuote clock database today

    let countDailyQuotes db =
        Seq.length(getDailyQuotes db)

    [<Fact>]
    member __.ConcurrencyTest() : Task<unit> =
        do
            reinitializeDatabase()
            use database = Storage.openDatabase config
            database.ReadWriteTransaction clearDailyQuotes

        let concurrencyLevel = 20
        async {
            use database = Storage.openDatabase config
            let tasks =
                seq { 1 .. concurrencyLevel }
                |> Seq.map (fun _ -> async { return executeTransaction database })
            let! quotes = Async.Parallel tasks

            let count = database.ReadOnlyTransaction countDailyQuotes
            Assert.Equal (1, count)

            let successCount =
                quotes
                |> Seq.filter Option.isSome
                |> Seq.length
            Assert.Equal (concurrencyLevel, successCount)
        } |> Async.StartAsTask
