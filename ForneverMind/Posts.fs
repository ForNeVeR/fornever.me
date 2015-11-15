module ForneverMind.Posts

open System.IO

open Freya.Core
open Freya.Router.Lenses

open ForneverMind.Models

let postFilePath =
    let htmlExtension = ".html"
    freya {
        let! maybeName = Route.Atom_ "name" |> Freya.Lens.getPartial
        let fileName = maybeName |> Option.orElse ""
        let name = Path.GetFileNameWithoutExtension fileName
        let extension = Path.GetExtension fileName
        let filePath =
            if extension = htmlExtension
            then Path.Combine (Config.postsDirectory, name + ".markdown")
            else "not-found.markdown"
        if not <| Common.pathIsInsideDirectory Config.postsDirectory filePath then failwith "Invalid file name"
        return filePath
    } |> Freya.memo

let checkPostExists =
    freya {
        let! filePath = postFilePath
        return File.Exists filePath
    }

let lastModified =
    freya {
        let! filePath = postFilePath
        return Some <| File.GetLastWriteTime filePath
    }

let allPosts =
    Directory.GetFiles Config.postsDirectory
    |> Seq.map (fun filePath ->
        use reader = new StreamReader (filePath)
        Markdown.processMetadata filePath reader)
    |> Seq.sortByDescending (fun x -> x.Date)
    |> Seq.toArray
