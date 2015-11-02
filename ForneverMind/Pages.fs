module ForneverMind.Pages

open System.Text

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http

let handlePage templateName model _ =
    freya {
        let! content = Freya.fromAsync (Templates.render templateName) model
        return
            {
                Description =
                    {
                        Charset = Some Charset.Utf8
                        Encodings = None
                        MediaType = Some MediaType.Html
                        Languages = None
                    }
                Data = Encoding.UTF8.GetBytes content
            }
    }

let private page templateName =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        handleOk (handlePage templateName None)
    } |> FreyaMachine.toPipeline

let index = page "Index"
