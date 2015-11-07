module ForneverMind.Posts

open System.IO

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http
open Freya.Router.Lenses

let private postFileName =
    let htmlExtension = ".html"
    freya {
        let! maybeName = Route.Atom_ "name" |> Freya.Lens.getPartial
        let fileName = maybeName |> Option.orElse ""
        let name = Path.GetFileNameWithoutExtension fileName
        let extension = Path.GetExtension name
        let templateName = if extension = htmlExtension then name else name + extension
        let filePath = Path.Combine (Config.postsDirectory, templateName + ".markdown")
        if not <| Common.pathIsInsideDirectory Config.postsDirectory filePath then failwith "Invalid file name"
        return filePath
    } |> Freya.memo

let private checkPostExists =
    freya {
        let! fileName = postFileName
        return File.Exists fileName
    }

let private handlePost state =
    freya {
        let! fileName = postFileName
        let! post = Freya.fromAsync Markdown.render fileName
        return! Pages.handlePage "Post" (Some post) state
    }

let post =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        exists checkPostExists

        handleOk handlePost
    } |> FreyaMachine.toPipeline

