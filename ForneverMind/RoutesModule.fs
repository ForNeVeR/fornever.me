// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open Freya.Routers.Uri.Template

type RoutesModule(pages: PagesModule) =
    let router =
        freyaRouter {
            resource "/{language}/posts/{name}" pages.Post
            resource "/{language}/" pages.Index // TODO: Migrate this
        }

    member __.Router = router
