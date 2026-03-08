// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open Freya.Core
open Freya.Machines.Http
open Freya.Optics.Http
open Freya.Types.Http
open Freya.Types.Uri

open ForneverMind.Core

type PagesModule(posts : PostsModule, markdown : MarkdownModule) =

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

    let indexPostCount = 20

    let postsModificationDate posts =
        posts
        |> Freya.map (Array.tryHead >> Option.map (fun p -> p.Date))

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

    member __.RedirectToDefaultLanguageIndex : HttpMachine = redirectToDefaultLanguageIndex
