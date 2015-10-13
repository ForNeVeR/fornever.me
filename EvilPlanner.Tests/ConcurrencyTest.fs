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
            let! quote = getQuote today
            ignore quote
        }

    let countDailyQuotes () =
        async {
            use context = new EvilPlannerContext ()
            return! countAsync context.DailyQuotes
        }

    [<Test>]
    member __.ConcurrencyTest () : unit =
        async {
            do! clearDailyQuotes ()

            let tasks =
                seq { 1 .. 20 }
                |> Seq.map (fun _ -> executeTransaction ())
            do! Async.Ignore <| Async.Parallel tasks

            let! count = countDailyQuotes ()
            return count = 1
        } |> Async.RunSynchronously |> ignore
