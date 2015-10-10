module EvilPlanner.Backend.Common

open System.Text

open Arachne.Http
open Arachne.Http.Cors
open Chiron
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http
open Freya.Machine.Extensions.Http.Cors

let private utf8 = Freya.init [ Charset.Utf8 ]
let private json = Freya.init [ MediaType.Json ]

let private corsOrigins = Freya.init AccessControlAllowOriginRange.Any
let private corsHeaders = Freya.init [ "accept"; "content-type" ]

let get = Freya.init [ GET ]

let machine =
    freyaMachine {
        using http
        using httpCors
        charsetsSupported utf8
        corsHeadersSupported corsHeaders
        corsOriginsSupported corsOrigins
        mediaTypesSupported json
    }

let inline resource obj =
    {
        Description =
            {
                Charset = Some Charset.Utf8
                Encodings = None
                MediaType = Some MediaType.Json
                Languages = None
            }
        Data = (Json.serialize >> Json.format >> Encoding.UTF8.GetBytes) obj
    }
