module EvilPlanner.Logic.QuoteLogic

open System
open System.Data
open System.Data.Entity
open System.Data.SqlClient

open EvilPlanner.Logic.DatabaseExtensions
open EvilPlanner.Data
open EvilPlanner.Data.Entities

let private getDailyQuote (context : EvilPlannerContext) date =
    query {
        for q in context.DailyQuotes do
        where (q.Date = date)
        select q.Quotation
    } |> singleOrDefaultAsync

let private getDailyQuoteTablockx (context : EvilPlannerContext) (date : DateTime) =
    context.Database.SqlQuery<Quotation>("
select q.*
from DailyQuotes dq with (tablockx)
join Quotations q on q.Id = dq.Quotation_Id
where dq.Date = @date
", SqlParameter("date", date))
    |> singleOrDefaultAsync

let private createQuote (context : EvilPlannerContext) (transaction : DbContextTransaction) date =
    async {
        // Retry the query with exclusive table lock in case concurrent request has already created the quote
        let! dailyQuote = getDailyQuoteTablockx context date
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
                let! quote = createQuote context transaction today
                return Some quote
    }

let getTodayQuote () : Async<Quotation> =
    let today = DateTime.UtcNow.Date
    async {
        let! quote = getQuote today
        return Option.get quote
    }
