module ForneverMind.Pages

open System
open System.Text

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http

open ForneverMind.Models

let handlePage templateName freyaModel _ =
    freya {
        let! model = freyaModel
        let! language = Common.routeLanguage
        let! content = Freya.fromAsync (Templates.render language templateName) model
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

let private lastModificationDate templateName =
    freya {
        let! language = Common.routeLanguage
        let date = Templates.lastModificationDate language templateName
        return Common.dateTimeToSeconds date
    }

let private page templateName model additionalModificationDate =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        lastModified (freya {
                          let! modificationDate = additionalModificationDate
                          let! templateModificationDate = lastModificationDate templateName
                          return max templateModificationDate (Option.orElse DateTime.MinValue modificationDate)
                      })
        handleOk (handlePage templateName model)
    } |> FreyaMachine.toPipeline

let handlePost state =
    freya {
        let! fileName = Posts.postFilePath
        let! post = Freya.fromAsync Markdown.render fileName
        return! handlePage "Post" (Freya.init <| Some post) state
    }

let private indexPostCount = 20

let private postsModificationDate posts =
    posts
    |> Freya.map (Seq.tryHead >> Option.map (fun p -> p.Date))

let private pageWithPosts pageName freyaPosts =
    let model = Freya.map (fun posts -> Some { Posts = posts }) freyaPosts
    page pageName model (postsModificationDate freyaPosts)

let private getPosts =
    freya {
        let! language = Common.routeLanguage
        return Posts.allPosts language
    }

let private latestPosts count =
    freya {
        let! posts = getPosts
        return posts |> Seq.truncate count |> Seq.toArray
    }

let index =
    let posts = latestPosts indexPostCount
    pageWithPosts "Index" posts

let archive = pageWithPosts "Archive" getPosts
let contact = page "Contact" (Freya.init None) (Freya.init None)

let post =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        exists Posts.checkPostExists
        lastModified Posts.lastModified

        handleOk handlePost
    } |> FreyaMachine.toPipeline
