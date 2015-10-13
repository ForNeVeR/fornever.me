namespace EvilPlanner.Tests

open System

open NUnit.Framework

open EvilPlanner.Data
open EvilPlanner.Logic.DatabaseExtensions
open EvilPlanner.Logic.QuoteLogic

[<TestFixture>]
type public ConcurrencyTest () =
    do Migrations.Configuration.EnableAutoMigration ()

    let getDailyQuotes (context : EvilPlannerContext) =
        toListAsync context.DailyQuotes

    let clearDailyQuotes () =
        async {
            use context = new EvilPlannerContext ()
            let! quotes = getDailyQuotes context

            ignore <| context.DailyQuotes.RemoveRange quotes

            do! saveChangesAsync context
        }

    let executeTransaction () =
        async {
            let today = DateTime.UtcNow.Date
            return! getQuote today
        }

    let countDailyQuotes () =
        async {
            use context = new EvilPlannerContext ()
            return! countAsync context.DailyQuotes
        }

    [<Test>]
    member __.ConcurrencyTest () : unit =
        let concurrencyLevel = 20
        async {
            do! clearDailyQuotes ()

            let tasks =
                seq { 1 .. concurrencyLevel }
                |> Seq.map (fun _ -> executeTransaction ())
            let! quotes = Async.Parallel tasks

            let! count = countDailyQuotes ()
            Assert.IsTrue ((count = 1))

            let successCount =
                quotes
                |> Seq.filter (fun q -> Option.isSome q)
                |> Seq.length
            Assert.AreEqual (concurrencyLevel, successCount)
        } |> Async.RunSynchronously |> ignore
