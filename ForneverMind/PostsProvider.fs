// SPDX-FileCopyrightText: 2017-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.IO
open ForneverMind.Core
open TruePath
open TruePath.SystemIo

type PostsProvider(config: ConfigurationModule, markdown: MarkdownModule) =

    interface IPostsProvider with
        member _.AllPosts language =
            let directory = config.PostsPath / language
            if not <| FileSystem.IsPathInsideDirectory(config.PostsPath, directory)
            then failwithf "Access error"

            if not (directory.ExistsDirectory()) then Array.empty
            else
                directory.GetFiles()
                |> Seq.map (fun filePath ->
                    use stream = new FileStream(filePath, FileMode.Open)
                    use reader = new StreamReader(stream)
                    markdown.ProcessMetadata(AbsolutePath filePath, reader))
                |> Seq.sortByDescending _.Date
                |> Seq.toArray
