// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System
open System.Text

open Freya.Core
open Freya.Machines.Http
open Freya.Optics.Http
open Freya.Types.Http
open Freya.Types.Uri

open ForneverMind.Core
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

    let redirectToDefaultLanguageIndex =
        freyaMachine {
            including Common.machine
            methods Common.methods
            exists false
            movedPermanently true
            handleMovedPermanently (freya {
                let url = sprintf "/%s/" Common.defaultLanguage
                do! Freya.Optic.set Response.Headers.location_ (Some(Location(UriReference.parse url)))
                return Representation.empty
            })
        }

    member __.Index = index
    member __.RedirectToDefaultLanguageIndex : HttpMachine = redirectToDefaultLanguageIndex
