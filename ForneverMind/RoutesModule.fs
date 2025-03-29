// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open Freya.Routers.Uri.Template

type RoutesModule(pages: PagesModule, rss: RssModule) =
    let router =
        freyaRouter {
            resource "/{language}/posts/{name}" pages.Post
            resource "/{language}/" pages.Index // TODO: Migrate this
            resource "/{language}/archive.html" pages.Archive
            resource "/{language}/contact.html" pages.Contact
            resource "/{language}/rss.xml" rss.Feed
            resource "/{language}/talks.html" pages.Talks
            resource "/rss.xml" rss.Feed
        }

    member __.Router = router
