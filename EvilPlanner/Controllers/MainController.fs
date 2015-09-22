namespace EvilPlanner.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open EvilPlanner.Data

type HomeController() =
    inherit Controller()
    member this.Index() = 
        use context = new EvilPlannerContext()
        let count = query { for q in context.Quotations do count }
        let toSkip = Random().Next count
        let quotation =
            query { 
                for q in context.Quotations do
                sortBy q.Id
                skip toSkip
                head
            }
        this.View("Quotation", quotation)

