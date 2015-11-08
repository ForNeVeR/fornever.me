module ForneverMind.Pages

open System.Text

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http

open ForneverMind.Models

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

let private page templateName model =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        handleOk (handlePage templateName model)
    } |> FreyaMachine.toPipeline

let handlePost state =
    freya {
        let! fileName = Posts.postFileName
        let! post = Freya.fromAsync Markdown.render fileName
        return! handlePage "Post" (Some post) state
    }

let indexPostCount = 20

let index =
    let posts = Posts.allPosts |> Seq.truncate indexPostCount |> Seq.toArray
    let model = { Posts = posts }
    page "Index" <| Some model

let post =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        exists Posts.checkPostExists

        handleOk handlePost
    } |> FreyaMachine.toPipeline

