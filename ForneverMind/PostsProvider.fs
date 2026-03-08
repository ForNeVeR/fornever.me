// SPDX-FileCopyrightText: 2017-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.IO
open ForneverMind.Core

type PostsProvider(config: ConfigurationModule, markdown: MarkdownModule) =

    interface IPostsProvider with
        member _.AllPosts language =
            let directory = Path.Combine(config.PostsPath, language)
            if not <| Common.pathIsInsideDirectory config.PostsPath directory then failwithf "Access error"
            if not (Directory.Exists directory) then Array.empty
            else
                Directory.GetFiles directory
                |> Seq.map (fun filePath ->
                    use stream = new FileStream(filePath, FileMode.Open)
                    use reader = new StreamReader(stream)
                    markdown.ProcessMetadata(filePath, reader))
                |> Seq.sortByDescending (fun x -> x.Date)
                |> Seq.toArray

