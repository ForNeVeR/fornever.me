// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.IO

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

type ConfigurationModule(app: IWebHostEnvironment, config: IConfigurationRoot) =
    let root = app.ContentRootPath

    member __.BaseUrl = config.["baseUrl"]
    member __.PostsPath = Path.Combine(root, "posts")
    member __.ViewsPath = Path.Combine(root, "views")
    member __.ServerJsPath = Path.Combine(root, "server.js")
