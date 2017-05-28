namespace ForneverMind

open System.Text

open Freya.Core
open Freya.Machines.Http
open Freya.Types.Http
open Freya.Routers.Uri
open Freya.Routers.Uri.Template
open Freya.Optics.Http

open ForneverMind.Models

type PagesModule(posts : PostsModule, templates : TemplatingModule, markdown : MarkdownModule) =
    let handlePage templateName model _ =
        freya {
            let! content = Freya.fromAsync (templates.Render templateName model)
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

    let lastModificationDate = templates.LastModificationDate

    let page templateName model modificationDate =
        let lastModificationDate = defaultArg modificationDate (lastModificationDate templateName)
        freyaMachine {
            including Common.machine
            methodsSupported Common.methods
            lastModified (Common.initLastModified lastModificationDate)
            handleOk (handlePage templateName model)
        }

    let handlePost state =
        freya {
            let! fileName = posts.PostFilePath
            let! post = Freya.fromAsync (markdown.Render fileName)
            return! handlePage "Post" (Some post) state
        }

    let indexPostCount = 20

    let pageWithPostsModificationDate pageName posts =
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
        let posts = posts.AllPosts |> Seq.truncate indexPostCount |> Seq.toArray
        pageWithPosts "Index" posts

    let archive = pageWithPosts "Archive" posts.AllPosts
    let contact = page "Contact" None None
    let talks = page "Talks" None None

    let shouldReturn404 =
        freya {
            let! url = Request.path_ |> Freya.Optic.get
            return url = "/404.html"
        }

    let notFoundHandler = handlePage "404" None

    let notFound =
        freyaMachine {
            including Common.machine
            methodsSupported Common.methods
            exists shouldReturn404
            handleNotFound notFoundHandler
            handleNotFound notFoundHandler
        }

    let error = page "Error" None None

    let post =
        freyaMachine {
            including Common.machine
            methodsSupported Common.methods
            exists posts.CheckPostExists
            lastModified (posts.PostLastModified <| lastModificationDate "Post")

            handleOk handlePost
            handleNotFound notFoundHandler
        }

    member __.Post = post
    member __.Index = index
    member __.Archive = archive
    member __.Contact = contact
    member __.Error = error
    member __.Talks = talks
    member __.NotFound = notFound
