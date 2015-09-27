module EvilPlanner.Logic.Quotations

open System
open EvilPlanner.Data

let getTodayQuote (context : EvilPlannerContext) =
    let count = query { for q in context.Quotations do count }
    let toSkip = Random().Next count    
    query { 
        for q in context.Quotations do
        where (q.Id > 0L)
        skip toSkip
        head
    }
