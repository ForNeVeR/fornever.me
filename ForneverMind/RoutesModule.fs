// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open Freya.Routers.Uri.Template

type RoutesModule(pages: PagesModule) =
    let router =
        freyaRouter {
            () // All routes have been migrated to ASP.NET Core
        }

    member __.Router = router
