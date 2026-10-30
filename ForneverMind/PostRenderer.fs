// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.Collections.Concurrent
open System.IO
open System.Text
open System.Threading.Tasks

open ForneverMind.Core
open TruePath

type PostRenderer(config: ConfigurationModule, markdown: MarkdownModule) =
    let postCache = ConcurrentDictionary()

    interface IPostRenderer with
        member _.PostsPath = config.PostsPath
        member _.Render(filePath: AbsolutePath): Task<PostModel> =
            match postCache.TryGetValue(filePath) with
            | true, post -> Task.FromResult post
            | _ -> task {
                do! Task.Yield()
                use stream = new FileStream(filePath.Value, FileMode.Open)
                use reader = new StreamReader(stream, Encoding.UTF8)
                let result = markdown.ProcessReader(filePath, reader)
                postCache.[filePath] <- result
                return result
            }
