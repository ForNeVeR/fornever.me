module EvilPlanner.Backend.Common

open System.Text

open Chiron
open Freya.Core
open Freya.Machines.Http
open Freya.Types.Http
open Freya.Types.Uri

let private utf8 = Freya.init [ Charset.Utf8 ]
let private json = Freya.init [ MediaType.Json ]

let get = Freya.init [ GET ]

let machine =
    freyaMachine {
        availableCharsets utf8
        availableMediaTypes json
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
