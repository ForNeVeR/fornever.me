module ForneverMind.Posts

open System.IO
open System.Text

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http
open Freya.Router.Lenses

let private postFileName =
    freya {
        let! maybeName = Route.Atom_ "name" |> Freya.Lens.getPartial
        let name = maybeName |> Option.orElse ""
        let fileName = Path.Combine (Config.postsDirectory, name + ".markdown") // TODO: Strip any special chars such as dots and especially slashes
        return fileName
    } |> Freya.memo

let private checkPostExists =
    freya {
        let! fileName = postFileName
        return File.Exists fileName
    }

let private handlePost _ =
    freya {
        let! fileName = postFileName
        let! content = Freya.fromAsync Markdown.render fileName
        let! page = Freya.fromAsync (Templates.render "Post") <| Some content // TODO: Add this template
        return
            {
                Description =
                    {
                        Charset = Some Charset.Utf8
                        Encodings = None
                        MediaType = Some MediaType.Html
                        Languages = None
                    }
                Data = Encoding.UTF8.GetBytes page
            }
    }

let post =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        exists checkPostExists
        handleOk (handlePost)
    } |> FreyaMachine.toPipeline

