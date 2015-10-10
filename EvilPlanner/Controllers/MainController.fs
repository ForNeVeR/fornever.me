namespace EvilPlanner.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax

open EvilPlanner.Data
open EvilPlanner.Data.Entities
open EvilPlanner.Logic

type HomeController() =
    inherit Controller()

    member this.Index() =
        async {
            use context = new EvilPlannerContext()
            let! quotation = QuoteLogic.getTodayQuote context
            return this.quotationView  quotation
        } |> Async.StartAsTask

    member private this.quotationView (model : Quotation) =
        this.View("Quotation", model)
