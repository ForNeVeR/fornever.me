﻿module EvilPlanner.Backend.Quotes

open System
open System.Globalization
open System.Text

open Chiron
open Chiron.Operators
open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template

open EvilPlanner.Core
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
        let! value = Freya.getLensPartial <| Route.Atom_ "date"
        return DateTime.ParseExact(
            Option.get value,
            isoDateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal ||| DateTimeStyles.AdjustToUniversal)
    }

let private findQuoteByDate database =
    freya {
        let getQuote = fun d -> async { return QuoteLogic.getQuote database d }
        let! date = date
        let! dbQuote = Freya.fromAsync (getQuote date)
        return dbQuote |> Option.map Quote
    } |> Freya.memo

let private checkQuoteByDateExists database =
    freya {
        let! quote = findQuoteByDate database
        return Option.isSome quote
    }

let private handleQuoteFound database _ =
    freya {
        let! quote = findQuoteByDate database
        return Common.resource quote
    }

let private quoteByDate database =
    freyaMachine {
        including Common.machine
        exists (checkQuoteByDateExists database)
        methods Common.get
        handleOk (handleQuoteFound database)
    }

let router(database : Storage.Database) : UriTemplateRouter =
     freyaRouter {
        resource "/quote/{date}" (quoteByDate database)
     }
