namespace EvilPlanner.Backend

open System.IO
open System.Text
open Freya.Core
open Arachne.Http
open Arachne.Language
open Chiron
open Freya.Core
open Freya.Core.Operators
open Freya.Lenses.Http
open Freya.Machine
open Freya.Machine.Extensions.Http
open System
open Arachne.Http
open Arachne.Http.Cors
open Arachne.Uri.Template
open Freya.Core
open Freya.Core.Operators
open Freya.Machine.Extensions.Http.Cors
open Freya.Machine.Router
open Freya.Router
open Microsoft.Owin

type Backend () =
    let en = Freya.init [ LanguageTag.Parse "en" ]
    let json = Freya.init [ MediaType.Json ]
    let utf8 = Freya.init [ Charset.Utf8 ]

    let corsOrigins =
        freya {
            return AccessControlAllowOriginRange.Any }

    let corsHeaders =
        freya {
            return [ "accept"
                     "content-type" ] }

    let common =
        freyaMachine {
            using http
            using httpCors
            charsetsSupported utf8
            corsHeadersSupported corsHeaders
            corsOriginsSupported corsOrigins
            languagesSupported en
            mediaTypesSupported json }

    let handler _ =
        freya {
            return
                { Description =
                    { Charset = Some Charset.Utf8
                      Encodings = None
                      MediaType = Some MediaType.Json
                      Languages = Some [ LanguageTag.Parse "en" ] }
                  Data = (Json.serialize >> Json.format >> Encoding.UTF8.GetBytes) "Hello World" }
        }


    let hello =
        freyaMachine {
            including common
            corsMethodsSupported (Freya.init [ DELETE
                                               GET
                                               OPTIONS
                                               POST ])
            methodsSupported (Freya.init [ DELETE
                                           GET
                                           OPTIONS
                                           POST ])
            handleOk handler } |> FreyaMachine.toPipeline

    let routes =
        freyaRouter {
            resource (UriTemplate.Parse "/") hello
        } |> FreyaRouter.toPipeline

    member __.Configuration () =
        OwinAppFunc.ofFreya routes

[<assembly: OwinStartup(typeof<Backend>)>]
()
