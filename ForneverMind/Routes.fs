module ForneverMind.Routes

open Arachne.Uri.Template

open Freya.Machine.Router
open Freya.Router

let router =
     freyaRouter {
        resource (UriTemplate.Parse "/main") Less.main

        resource (UriTemplate.Parse "/") Pages.index
     } |> FreyaRouter.toPipeline
