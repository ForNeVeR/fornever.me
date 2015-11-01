module ForneverMind.Pages

open System.Text

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http

let private handlePage name _ =
    freya {
        let! index = Freya.fromAsync (Templates.render name) None
        return
            {
                Description =
                    {
                        Charset = Some Charset.Utf8
                        Encodings = None
                        MediaType = Some MediaType.Html
                        Languages = None
                    }
                Data = Encoding.UTF8.GetBytes index
            }
    }

let private page name =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        handleOk (handlePage name)
    } |> FreyaMachine.toPipeline

let index = page "Index"
