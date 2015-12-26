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

let private lastModificationDate = Templates.lastModificationDate

let private page templateName model modificationDate =
    let lastModificationDate = defaultArg modificationDate (lastModificationDate templateName)
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        lastModified (Common.initLastModified lastModificationDate)
        handleOk (handlePage templateName model)
    } |> FreyaMachine.toPipeline

let handlePost state =
    freya {
        let! fileName = Posts.postFilePath
        let! post = Freya.fromAsync Markdown.render fileName
        return! handlePage "Post" (Some post) state
    }

let private indexPostCount = 20

let private pageWithPostsModificationDate pageName posts =
    let pageLastModified = lastModificationDate pageName
    let lastPostAdded =
        posts
        |> Seq.tryHead
        |> Option.map (fun p -> p.Date)
    Some (match lastPostAdded with
          | Some date when date > pageLastModified -> date
          | _ -> pageLastModified)

let pageWithPosts pageName posts =
    let model = { Posts = posts }
    page pageName (Some model) (pageWithPostsModificationDate pageName posts)

let index =
    let posts = Posts.allPosts |> Seq.truncate indexPostCount |> Seq.toArray
    pageWithPosts "Index" posts

let archive = pageWithPosts "Archive" Posts.allPosts
let contact = page "Contact" None None

let post =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        exists Posts.checkPostExists
        lastModified Posts.lastModified

        handleOk handlePost
    } |> FreyaMachine.toPipeline
