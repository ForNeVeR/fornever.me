module ForneverMind.Posts

open System.IO

open Freya.Core
open Freya.Routers.Uri.Template

open ForneverMind.Models

let postFilePath =
    let htmlExtension = ".html"
    freya {
        let! maybeName = Route.atom_ "name" |> Freya.Optic.get
        let fileName = maybeName |> Option.defaultValue ""
        let name = Path.GetFileNameWithoutExtension fileName
        let extension = Path.GetExtension fileName
        let filePath =
            if extension = htmlExtension
            then Path.Combine (Config.postsDirectory, name + ".md")
            else Path.Combine (Config.postsDirectory, "not-found.md")
        if not <| Common.pathIsInsideDirectory Config.postsDirectory filePath then failwith "Invalid file name"
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
    Directory.GetFiles Config.postsDirectory
    |> Seq.map (fun filePath ->
        use stream = new FileStream(filePath, FileMode.Open)
        use reader = new StreamReader(stream)
        Markdown.processMetadata filePath reader)
    |> Seq.sortByDescending (fun x -> x.Date)
    |> Seq.toArray
