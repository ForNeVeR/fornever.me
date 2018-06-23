namespace EvilPlanner.Tests

open System
open System.IO
open System.Reflection
open System.Threading.Tasks

open LiteDB
open Xunit

open EvilPlanner.Core
open EvilPlanner.Logic.QuoteLogic

type public ConcurrencyTest () =
    let config =
        let directory = AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
        { databasePath = Path.Combine(directory, "testDb.dat") }
    do Migrations.migrateDatabase config

    let dailyQuotes(db : LiteDatabase) = db.GetCollection<DailyQuote>("dailyQuotes")
    let getDailyQuotes(db : LiteDatabase) = (dailyQuotes db).FindAll()

    let clearDailyQuotes () =
        use db = Storage.openDatabase config
        let quotes = getDailyQuotes db
        ignore <| (dailyQuotes db).Delete(Query.All())

    let executeTransaction () =
        let today = DateTime.UtcNow.Date
        getQuote config today

    let countDailyQuotes () =
        use db = Storage.openDatabase config
        Seq.length(getDailyQuotes db)

    [<Fact>]
    member __.ConcurrencyTest() : Task<unit> =
        let concurrencyLevel = 20
        clearDailyQuotes()

        async {
            let tasks =
                seq { 1 .. concurrencyLevel }
                |> Seq.map (fun _ -> async { return executeTransaction() })
            let! quotes = Async.Parallel tasks

            let count = countDailyQuotes()
            Assert.Equal (1, count)

            let successCount =
                quotes
                |> Seq.filter Option.isSome
                |> Seq.length
            Assert.Equal (concurrencyLevel, successCount)
        } |> Async.StartAsTask
