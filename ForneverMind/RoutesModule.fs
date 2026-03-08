// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open Freya.Routers.Uri.Template

type RoutesModule(pages: PagesModule, rss: RssModule) =
    let router =
        freyaRouter {
            resource "/{language}/posts/{name}" pages.Post
            resource "/{language}/" pages.Index // TODO: Migrate this
            resource "/{language}/rss.xml" rss.Feed
            resource "/rss.xml" rss.Feed
        }

    member __.Router = router
