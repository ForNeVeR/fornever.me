namespace EvilPlanner.Tests

open System
open System.Threading.Tasks

open Xunit

open EvilPlanner.Data
open EvilPlanner.Logic.DatabaseExtensions
open EvilPlanner.Logic.QuoteLogic

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

    [<Fact>]
    member __.ConcurrencyTest () : Task<unit> =
        let concurrencyLevel = 20
        async {
            do! clearDailyQuotes ()

            let tasks =
                seq { 1 .. concurrencyLevel }
                |> Seq.map (fun _ -> executeTransaction ())
            let! quotes = Async.Parallel tasks

            let! count = countDailyQuotes ()
            Assert.Equal (1, count)

            let successCount =
                quotes
                |> Seq.filter Option.isSome
                |> Seq.length
            Assert.Equal (concurrencyLevel, successCount)
        } |> Async.StartAsTask
