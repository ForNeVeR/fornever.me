module EvilPlanner.Backend.Quotes

open System
open System.Globalization
open System.Text

open Arachne.Http
open Arachne.Uri.Template
open Chiron
open Chiron.Operators
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http
open Freya.Machine.Extensions.Http.Cors
open Freya.Machine.Router
open Freya.Router
open Freya.Router.Lenses

open EvilPlanner.Data
open EvilPlanner.Data.Entities
open EvilPlanner.Logic

type Quote (q : Quotation) =
    member public __.Model = q

    static member ToJson (quote : Quote) =
        let q = quote.Model

        Json.write "id" q.Id
        *> Json.write "text" q.Text
        *> Json.write "sourceUrl" q.SourceUrl
        *> Json.write "source" q.Source

let private isoDateFormat = "yyyy-MM-dd"

let private date =
    freya {
        let! value = Freya.getLensPartial <| Route.Atom_ "date"
        return DateTime.ParseExact (Option.get value, isoDateFormat, CultureInfo.InvariantCulture)
    }

let private getQuoteByDate _ =
    freya {
        let! date = date
        use context = new EvilPlannerContext ()
        let! dbQuote = (Freya.fromAsync QuoteLogic.getQuote) (context, date)
        let quote = Quote <| Option.get dbQuote
        return Common.resource quote
    }

let private quoteByDate =
    freyaMachine {
        including Common.machine
        corsMethodsSupported Common.get
        methodsSupported Common.get
        handleOk getQuoteByDate
    } |> FreyaMachine.toPipeline

let router =
     freyaRouter {
        resource (UriTemplate.Parse "/quote/{date}") quoteByDate
     } |> FreyaRouter.toPipeline
