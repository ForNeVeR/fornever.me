module EvilPlanner.Logic.QuoteLogic

open System
open System.Data
open System.Data.Entity
open System.Data.SqlClient

open EvilPlanner.Logic.DatabaseExtensions
open EvilPlanner.Data
open EvilPlanner.Data.Entities
open System.Data.Entity.Core
open System.Data.Entity.Infrastructure

let private getDailyQuote (context : EvilPlannerContext) date =
    query {
        for q in context.DailyQuotes do
        where (q.Date = date)
        select q.Quotation
    } |> singleOrDefaultAsync

/// Creates quote for the current date. Can throw an UpdateException in case when the quote was
/// already created by the concurrent query.
let private createQuote (context : EvilPlannerContext) (transaction : DbContextTransaction) date =
    async {
        // TODO: Optimize this randomization
        let count = query { for q in context.Quotations do count }
        let toSkip = Random().Next count
        let! quotation =
            query {
                for q in context.Quotations do
                sortBy q.Id
                skip toSkip
            } |> headAsync

        let dailyQuote = DailyQuote(Date = date, Quotation = quotation)
        context.DailyQuotes.Add dailyQuote |> ignore

        do! saveChangesAsync context
        transaction.Commit()

        return quotation
    }

let getQuote (date : DateTime) : Async<Quotation option> =
    let today = DateTime.UtcNow.Date
    async {
        use context = new EvilPlannerContext ()
        if today <> date
        then
            return! getDailyQuote context date
        else
            let! currentQuote = getDailyQuote context today

            match currentQuote with
            | Some(q) -> return Some q
            | None    ->
                use transaction = context.Database.BeginTransaction()
                let! quoteOrError = Async.Catch <| createQuote context transaction today
                let quote =
                    match quoteOrError with
                    | Choice1Of2 q -> async { return Some q }
                    | Choice2Of2 (SqlException ex) ->
                        // Retry the select in case another transaction has been already created
                        // the quote:
                        getDailyQuote context today
                    | Choice2Of2 error -> raise error

                return! quote
    }
