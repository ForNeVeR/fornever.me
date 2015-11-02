module ForneverMind.Routes

open Arachne.Uri.Template

open Freya.Machine.Router
open Freya.Router

let router =
     freyaRouter {
        resource (UriTemplate.Parse "/css/main.css") Less.main
        resource (UriTemplate.Parse "/posts/{name}.html") Posts.post

        resource (UriTemplate.Parse "/") Pages.index
     } |> FreyaRouter.toPipeline
