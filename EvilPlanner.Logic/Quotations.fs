module EvilPlanner.Logic.Quotations

open System
open System.Data
open System.Data.Entity

open EvilPlanner.Data
open EvilPlanner.Data.Entities

let private getDailyQuote (context : EvilPlannerContext) date =
    query {
        for q in context.DailyQuotes do
        where (q.Date = date)
        select q.Quotation
        headOrDefault
    } |> Option.ofObj

let createQuote (context : EvilPlannerContext) (transaction : DbContextTransaction) date =
    // Retry the query in a serializable transaction in case concurrent request has already created the quote
    match getDailyQuote context date with
    | Some(q) -> q
    | None ->
        // TODO: Optimize this randomization
        let count = query { for q in context.Quotations do count }
        let toSkip = Random().Next count    
        let quotation =
            query { 
                for q in context.Quotations do
                sortBy q.Id
                skip toSkip
                head
            }
        
        let dailyQuote = DailyQuote(Date = date, Quotation = quotation)
        context.DailyQuotes.Add dailyQuote |> ignore

        context.SaveChanges() |> ignore
        transaction.Commit()

        quotation

let getTodayQuote (context : EvilPlannerContext) =
    let today = DateTime.UtcNow.Date
    let currentQuote = getDailyQuote context today        

    match currentQuote with
    | Some(q) -> q
    | None    ->
        use transaction = context.Database.BeginTransaction(IsolationLevel.Serializable)
        createQuote context transaction today
