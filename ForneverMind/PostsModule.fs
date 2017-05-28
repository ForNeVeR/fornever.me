namespace ForneverMind

open System.IO

open Freya.Core
open Freya.Routers.Uri.Template

open ForneverMind.Models

type PostsModule (config : ConfigurationModule, markdown : MarkdownModule) =
    let postFilePath =
        let htmlExtension = ".html"
        freya {
            let! maybeName = Route.atom_ "name" |> Freya.Optic.get
            let fileName = maybeName |> Option.defaultValue ""
            let name = Path.GetFileNameWithoutExtension fileName
            let extension = Path.GetExtension fileName
            let filePath =
                if extension = htmlExtension
                then Path.Combine(config.PostsPath, name + ".md")
                else Path.Combine(config.PostsPath, "not-found.md")
            if not <| Common.pathIsInsideDirectory config.PostsPath filePath then failwith "Invalid file name"
            return filePath
        } |> Freya.memo

    let checkPostExists =
        freya {
            let! filePath = postFilePath
            return File.Exists filePath
        }

    let postLastModified templateModificationDate =
        freya {
            let! filePath = postFilePath
            let postModificationDate = File.GetLastWriteTimeUtc filePath
            let lastModificationDate = max postModificationDate templateModificationDate
            return Common.dateTimeToSeconds lastModificationDate
        }

    let allPosts =
        Directory.GetFiles config.PostsPath
        |> Seq.map (fun filePath ->
            use stream = new FileStream(filePath, FileMode.Open)
            use reader = new StreamReader(stream)
            markdown.ProcessMetadata(filePath, reader))
        |> Seq.sortByDescending (fun x -> x.Date)
        |> Seq.toArray

    member __.PostFilePath = postFilePath
    member __.CheckPostExists = checkPostExists
    member __.PostLastModified = postLastModified
    member __.AllPosts = allPosts
