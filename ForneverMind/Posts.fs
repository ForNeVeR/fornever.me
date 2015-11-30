module ForneverMind.Posts

open System.IO

open Freya.Core
open Freya.Router

open ForneverMind.Models

let postFileName =
    let htmlExtension = ".html"
    freya {
        let! maybeName = Route.Atom_ "name" |> Freya.Lens.getPartial
        let fileName = maybeName |> Option.orElse ""
        let name = Path.GetFileNameWithoutExtension fileName
        let extension = Path.GetExtension fileName
        let filePath =
            if extension = htmlExtension
            then Path.Combine (Config.postsDirectory, name + ".md")
            else "not-found.md"
        if not <| Common.pathIsInsideDirectory Config.postsDirectory filePath then failwith "Invalid file name"
        return filePath
    } |> Freya.memo

let checkPostExists =
    freya {
        let! fileName = postFileName
        return File.Exists fileName
    }

let allPosts =
    Directory.GetFiles Config.postsDirectory
    |> Seq.map (fun filePath ->
        use reader = new StreamReader (filePath)
        Markdown.processMetadata filePath reader)
    |> Seq.sortByDescending (fun x -> x.Date)
    |> Seq.toArray
