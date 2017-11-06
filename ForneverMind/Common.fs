module ForneverMind.Common

open System
open System.IO

open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template
open Freya.Types.Http

let private utf8 = Freya.init [ Charset.Utf8 ]
let private html = Freya.init [ MediaType.Html ]

let rss = MediaType (Type "application", SubType "rss+xml", Parameters Map.empty)

let methods = Freya.init [ GET; HEAD ]

let defaultLanguage = "en"
let supportedLanguages = [| "en"; "ru" |]

let routeLanguage =
    Route.atom_ "language"
    |> Freya.Optic.get
    |> Freya.map Option.get
    |> Freya.memo
let routeLanguageOpt =
    Route.atom_ "language"
    |> Freya.Optic.get
    |> Freya.memo

let machine =
    freyaMachine {
        availableCharsets utf8
        availableMediaTypes html
    }

let pathIsInsideDirectory directory path =
    let fullDirectory = Path.GetFullPath directory
    let fullPath = Path.GetFullPath path
    fullPath.StartsWith fullDirectory

let dateTimeToSeconds (date : DateTime) =
    let d = date.ToUniversalTime ()
    DateTime(d.Year,
             d.Month,
             d.Day,
             d.Hour,
             d.Minute,
             d.Second,
             DateTimeKind.Utc)
