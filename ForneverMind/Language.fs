module ForneverMind.Language

open System

open Freya.Core
open Freya.Machines.Http
open Freya.Machines.Http.Machine.Specifications
open Freya.Optics.Http
open Freya.Routers.Uri.Template
open Freya.Types.Http
open Freya.Types.Language
open Freya.Types.Uri

let private query = Route.Atom_ "query" |> Freya.Lens.getPartial |> Freya.map Option.get |> Freya.memo

let internal getLanguage (AcceptLanguage languages) : string option =
    let getLanguageFromLocale (str : string) = str.Split('-').[0]

    let matchWithSupportedLanguage language =
        if Array.contains language Common.supportedLanguages
        then Some language
        else None

    let extractRangeLanguage locales =
        locales
        |> Seq.map getLanguageFromLocale
        |> Seq.map matchWithSupportedLanguage
        |> Seq.filter Option.isSome
        |> Seq.tryHead
        |> Option.defaultValue None

    let getWeight = function
    | AcceptableLanguage (_, Some (Weight w)) -> w
    | AcceptableLanguage (_, None) -> Double.MinValue

    let extractSupportedLanguage (AcceptableLanguage (range, _)) =
        match range with
        | Any -> Some Common.defaultLanguage
        | Range list -> extractRangeLanguage list

    languages
    |> Seq.sortByDescending getWeight
    |> Seq.map extractSupportedLanguage
    |> Seq.filter Option.isSome
    |> Seq.tryHead
    |> Option.defaultValue None

let private acceptLanguage =
    freya {
        let! headerLanguage = Freya.Lens.getPartial Request.Headers.acceptLanguage_
        let language =
            Option.bind getLanguage headerLanguage
            |> Option.defaultValue Common.defaultLanguage
        return language
    }

let private noLanguageInUrl =
    freya {
        let! query = query
        return query.StartsWith("/en/") || query.StartsWith("/ru/")
    }

let private handle =
    freya {
        let! query = query
        let! language = acceptLanguage
        let url = sprintf "/%s/%s" language query
        do! Freya.Lens.setPartial Response.Headers.Location_ (Location(UriReference.parse url))
        return Representation.empty
    }

let languageSelector =
    freyaMachine {
        including Common.machine
        methods Common.methods
        movedPermanently noLanguageInUrl
        handleMovedPermanently handle
    }
