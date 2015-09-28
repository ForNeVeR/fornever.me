module EvilPlanner.Logic.Quotations

open System
open System.Data
open System.Data.Entity

open EvilPlanner.Logic.DatabaseExtensions
open EvilPlanner.Data
open EvilPlanner.Data.Entities

let private getDailyQuote (context : EvilPlannerContext) date =
    query {
        for q in context.DailyQuotes do
        where (q.Date = date)
        select q.Quotation
    } |> headOrDefaultAsync

let private createQuote (context : EvilPlannerContext) (transaction : DbContextTransaction) date =
    async {
        // Retry the query in a serializable transaction in case concurrent request has already created the quote
        let! dailyQuote = getDailyQuote context date
        match dailyQuote with
        | Some(q) -> return q
        | None ->
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

let getTodayQuote (context : EvilPlannerContext) : Async<Quotation> =
    async {
        let today = DateTime.UtcNow.Date
        let! currentQuote = getDailyQuote context today        

        match currentQuote with
        | Some(q) -> return q
        | None    ->
            use transaction = context.Database.BeginTransaction(IsolationLevel.Serializable)
            return! createQuote context transaction today
    }
