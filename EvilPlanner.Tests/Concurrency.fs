module EvilPlanner.Tests.Concurrency

open EvilPlanner.Data
open EvilPlanner.Logic.DatabaseExtensions
open EvilPlanner.Logic.QuoteLogic

let private getDailyQuotes (context : EvilPlannerContext) =
    toListAsync context.DailyQuotes

let private clearDailyQuotes () =
    async {
        use context = new EvilPlannerContext ()
        let! quotes = getDailyQuotes context

        ignore <| context.DailyQuotes.RemoveRange quotes

        do! saveChangesAsync context
    }

let private executeTransaction () =
    async {
        use context = new EvilPlannerContext ()
        let! quote = getTodayQuote context
        ignore quote
    }

let private countDailyQuotes () =
    async {
        use context = new EvilPlannerContext ()
        return! countAsync context.DailyQuotes
    }

let test () =
    async {
        do! clearDailyQuotes ()

        let tasks =
            seq { 1 .. 20 }
            |> Seq.map (fun _ -> executeTransaction ())
        do! Async.Ignore <| Async.Parallel tasks

        let! count = countDailyQuotes ()
        return count = 1
    }
