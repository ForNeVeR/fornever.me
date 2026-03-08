// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open TruePath

type ConfigurationModule(app: IWebHostEnvironment, config: IConfigurationRoot) =
    let root = AbsolutePath app.ContentRootPath

    member _.BaseUrl = config["baseUrl"]
    member _.PostsPath = root / "posts"
    member _.ViewsPath = root / "views"
    member _.ServerJsPath = root / "server.js"
