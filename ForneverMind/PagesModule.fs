namespace ForneverMind

open System
open System.Text

open Freya.Core
open Freya.Machines.Http
open Freya.Optics.Http
open Freya.Routers.Uri
open Freya.Routers.Uri.Template
open Freya.Types.Http
open Freya.Types.Uri

open ForneverMind.Models

type PagesModule(posts : PostsModule, templates : TemplatingModule, markdown : MarkdownModule) =
    let handlePage language templateName freyaModel links =
        freya {
            let! model = freyaModel
            let! language = language
            let! content = Freya.fromAsync (templates.Render language templateName model links)
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

    let handleStaticPage language templateName model active =
        let produceLink (path : string) language =
            if active
            then sprintf "/%s/%s" language (path.Substring("/en".Length + 1))
            else ""
        freya {
            let! path = Freya.Optic.get Request.path_
            let links = {
                Russian = { IsActive = active; Link = produceLink path "ru" }
                English = { IsActive = active; Link = produceLink path "en" }
            }
            return! handlePage language templateName model links
        }

    let lastModificationDate templateName =
        freya {
            let! language = Common.routeLanguage
            let date = templates.LastModificationDate language templateName
            return Common.dateTimeToSeconds date
        }

    let isKnownLanguage =
        freya {
            let! language = Common.routeLanguageOpt
            let known =
                match language with
                | None -> false
                | Some lang ->
                    Array.contains lang Common.supportedLanguages
            return known
        }

    let notFoundHandler =
        let language =
            freya {
                let! language = Common.routeLanguageOpt
                let language =
                    language
                    |> Option.map (fun lang ->
                        if Array.contains lang Common.supportedLanguages
                        then lang
                        else Common.defaultLanguage)

                return Option.defaultValue Common.defaultLanguage language
            }
        handleStaticPage language "404" (Freya.init None) false

    let page templateName model additionalModificationDate =
        freyaMachine {
            including Common.machine
            methods Common.methods
            exists isKnownLanguage
            lastModified (freya {
                              let! modificationDate = additionalModificationDate
                              let! templateModificationDate = lastModificationDate templateName
                              return max templateModificationDate (Option.defaultValue DateTime.MinValue modificationDate)
                          })
            handleOk (handleStaticPage Common.routeLanguage templateName model true)
            handleNotFound notFoundHandler
        }

    let handlePost =
        freya {
            let! fileName = posts.PostFilePath
            let! post = Freya.fromAsync (markdown.Render fileName)
            let! links = posts.CurrentPostLinks
            return! handlePage Common.routeLanguage "Post" (Freya.init <| Some post) links
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

    let notFound =
        let pageRequestedExplicitly =
            freya {
                let supportedUrls =
                    Common.supportedLanguages
                    |> Seq.map (sprintf "/%s/404.html")
                let! url = Request.path_ |> Freya.Optic.get
                return Seq.contains url supportedUrls
            }
        freyaMachine {
            including Common.machine
            methods Common.methods
            exists pageRequestedExplicitly
            handleNotFound notFoundHandler
            handleOk notFoundHandler
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

    let redirectToDefaultLanguageIndex =
        freyaMachine {
            including Common.machine
            methods Common.methods
            exists false
            movedPermanently true
            handleMovedPermanently (freya {
                let url = sprintf "/%s/" Common.defaultLanguage
                do! Freya.Lens.setPartial Response.Headers.Location_ (Location(UriReference.parse url))
                return Representation.empty
            })
        }

    member __.Post = post
    member __.Index = index
    member __.Archive = archive
    member __.Contact = contact
    member __.Error = error
    member __.Talks = talks
    member __.NotFound = notFound
    member __.RedirectToDefaultLanguageIndex : HttpMachine = redirectToDefaultLanguageIndex
