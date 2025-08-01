// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.IO

open ForneverMind.Core
open Freya.Core
open Freya.Routers.Uri.Template

open ForneverMind.Models

type PostsModule (config : ConfigurationModule, markdown : MarkdownModule) =
    let postFilePathForLanguage language =
        let htmlExtension = ".html"
        freya {
            let! maybeName = Route.atom_ "name" |> Freya.Optic.get
            let fileName = maybeName |> Option.defaultValue ""
            let name = Path.GetFileNameWithoutExtension fileName
            let extension = Path.GetExtension fileName
            let filePath =
                if extension = htmlExtension
                then Path.Combine(config.PostsPath, language, name + ".md")
                else Path.Combine(config.PostsPath, language, "not-found.md")
            if not <| Common.pathIsInsideDirectory config.PostsPath filePath then failwith "Invalid file name"
            return filePath
        }

    let postFilePath =
        freya {
            let! language = Common.routeLanguage
            return! postFilePathForLanguage language
        } |> Freya.memo

    let checkPostExists =
        freya {
            let! filePath = postFilePath
            return File.Exists filePath
        }

    let checkPostExistsForLanguage language =
        freya {
            let! filePath = postFilePathForLanguage language
            return File.Exists filePath
        }

    let currentPostLinks =
        let prepareLink language =
            freya {
                let! maybeName = Route.atom_ "name" |> Freya.Optic.get
                let name = maybeName |> Option.defaultValue ""
                let! active = checkPostExistsForLanguage language
                return { IsActive = active; Link = sprintf "/%s/posts/%s" language name }
            }
        freya {
            let! english = prepareLink "en"
            let! russian = prepareLink "ru"
            return { English = english; Russian = russian }
        }

    let postLastModified templateModificationDate =
        freya {
            let! filePath = postFilePath
            let postModificationDate = File.GetLastWriteTimeUtc filePath
            let serverJsModificationDate = File.GetLastWriteTimeUtc config.ServerJsPath
            let! templateModificationDate = templateModificationDate
            let lastModificationDate = max postModificationDate templateModificationDate
            return Common.dateTimeToSeconds lastModificationDate
        }

    let allPosts language =
        let directory = Path.Combine (config.PostsPath, language)
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

    member __.PostFilePath = postFilePath
    member __.CheckPostExists = checkPostExists
    member __.CurrentPostLinks : Freya<LanguageLinks> = currentPostLinks
    member __.PostLastModified = postLastModified
    member __.AllPosts = allPosts
