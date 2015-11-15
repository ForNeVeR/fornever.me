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
        let! fileName = Posts.postFilePath
        let! post = Freya.fromAsync Markdown.render fileName
        return! handlePage "Post" (Some post) state
    }

let private indexPostCount = 20

let archive =
    let model = { Posts = Posts.allPosts }
    page "Archive" <| Some model

let contact = page "Contact" None

let index =
    let posts = Posts.allPosts |> Seq.truncate indexPostCount |> Seq.toArray
    let model = { Posts = posts }
    page "Index" <| Some model

let post =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        exists Posts.checkPostExists
        lastModified Posts.lastModified

        handleOk handlePost
    } |> FreyaMachine.toPipeline
