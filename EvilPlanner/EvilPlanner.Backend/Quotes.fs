module EvilPlanner.Backend.Quotes

open System
open System.Globalization
open System.Text

open Chiron
open Chiron.Operators
open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template

open EvilPlanner.Core
open EvilPlanner.Core.Storage
open EvilPlanner.Logic

type Quote (q : Quotation) =
    member public __.Model = q

    static member ToJson (quote : Quote) =
        let q = quote.Model

        Json.write "id" q.id
        *> Json.write "text" q.text
        *> Json.write "sourceUrl" q.sourceUrl
        *> Json.write "source" q.source

let private isoDateFormat = "yyyy-MM-dd"

let private date =
    freya {
        let! value = Freya.Optic.get <| Route.atom_ "date"
        return DateOnly.ParseExact(
            Option.get value,
            isoDateFormat,
            CultureInfo.InvariantCulture)
    }

let private findQuoteByDate clock database =
    freya {
        let getQuote = fun d -> async { return QuoteLogic.getQuote clock database d }
        let! date = date
        let! dbQuote = Freya.fromAsync (getQuote date)
        return dbQuote |> Option.map Quote
    } |> Freya.memo

let private checkQuoteByDateExists clock database =
    freya {
        let! quote = findQuoteByDate clock database
        return Option.isSome quote
    }

let private handleQuoteFound clock database _ =
    freya {
        let! quote = findQuoteByDate clock database
        return Common.resource quote
    }

let quoteByDate (clock: IClock) (database: Database): HttpMachine =
    freyaMachine {
        including Common.machine
        exists (checkQuoteByDateExists clock database)
        methods Common.get
        handleOk (handleQuoteFound clock database)
    }
