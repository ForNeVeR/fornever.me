module ForneverMind.Routes

open System
open System.Globalization
open System.Text

open Arachne.Http
open Arachne.Uri.Template

open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http
open Freya.Machine.Extensions.Http.Cors
open Freya.Machine.Router
open Freya.Router
open Freya.Router.Lenses

let private handleIndex _ =
    freya {
        return
            {
                Description =
                    {
                        Charset = Some Charset.Utf8
                        Encodings = None
                        MediaType = Some MediaType.Html
                        Languages = None
                    }
                Data = "<h1>Hello World</h1>"B
            }
    }

let private index =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        handleOk handleIndex
    } |> FreyaMachine.toPipeline

let router =
     freyaRouter {
        resource (UriTemplate.Parse "/") index
     } |> FreyaRouter.toPipeline
