namespace ForneverMind

open System
open System.Text

open Freya.Core
open Freya.Machines.Http
open Freya.Types.Http
open Freya.Routers.Uri
open Freya.Routers.Uri.Template
open Freya.Optics.Http

open ForneverMind.Models

type PagesModule(posts : PostsModule, templates : TemplatingModule, markdown : MarkdownModule) =
    let handlePage templateName freyaModel _ =
        freya {
            let! model = freyaModel
            let! language = Common.routeLanguage
            let! content = Freya.fromAsync (templates.Render language templateName model)
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

    let lastModificationDate templateName =
        freya {
            let! language = Common.routeLanguage
            let date = templates.LastModificationDate language templateName
            return Common.dateTimeToSeconds date
        }

    let page templateName model additionalModificationDate =
        freyaMachine {
            including Common.machine
            methods Common.methods
            lastModified (freya {
                              let! modificationDate = additionalModificationDate
                              let! templateModificationDate = lastModificationDate templateName
                              return max templateModificationDate (Option.defaultValue DateTime.MinValue modificationDate)
                          })
            handleOk (handlePage templateName model)
        }

    let handlePost state =
        freya {
            let! fileName = posts.PostFilePath
            let! post = Freya.fromAsync (markdown.Render fileName)
            return! handlePage "Post" (Freya.init <| Some post) state
        }

    let indexPostCount = 20

    let postsModificationDate posts =
        posts
        |> Freya.map (Array.tryHead >> Option.map (fun p -> p.Date))

    let pageWithPosts pageName freyaPosts =
        let model = Freya.map (fun posts -> Some { Posts = posts }) freyaPosts
        page pageName model (postsModificationDate freyaPosts)

    let getPosts =
        freya {
            let! language = Common.routeLanguage
            return posts.AllPosts language
        }

    let latestPosts count =
        freya {
            let! posts = getPosts
            return posts |> Array.truncate count
        }

    let index =
        let posts = latestPosts indexPostCount
        pageWithPosts "Index" posts

    let archive = pageWithPosts "Archive" getPosts
    let contact = page "Contact" (Freya.init None) (Freya.init None)
    let talks = page "Talks" (Freya.init None) (Freya.init None)

    let shouldReturn404 =
        freya {
            let! url = Request.path_ |> Freya.Optic.get
            return url = "/404.html"
        }

    let notFoundHandler = handlePage "404" (Freya.init None)

    let notFound =
        freyaMachine {
            including Common.machine
            methods Common.methods
            exists shouldReturn404
            handleNotFound notFoundHandler
            handleNotFound notFoundHandler
        }

    let error = page "Error" (Freya.init None) (Freya.init None)

    let post =
        freyaMachine {
            including Common.machine
            methods Common.methods
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
