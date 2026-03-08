// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.IO
open System.Text
open System.Threading.Tasks

open ForneverMind.Core

type PostRenderer(config: ConfigurationModule, markdown: MarkdownModule) =
    interface IPostRenderer with
        member _.PostsPath = config.PostsPath
        member _.Render(filePath: string): Task<PostModel> =
            task {
                do! Task.Yield()
                use stream = new FileStream(filePath, FileMode.Open)
                use reader = new StreamReader(stream, Encoding.UTF8)
                return markdown.ProcessReader(filePath, reader)
            }
