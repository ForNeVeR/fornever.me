namespace EvilPlanner.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open EvilPlanner.Data
open EvilPlanner.Logic

type HomeController() =
    inherit Controller()
    member this.Index() = 
        use context = new EvilPlannerContext()
        let quotation = Quotations.getTodayQuote context
        this.View("Quotation", quotation)

