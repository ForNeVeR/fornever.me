module ForneverMind.Routes

open Arachne.Uri.Template

open Freya.Machine.Router
open Freya.Router

let router =
     freyaRouter {
        resource (UriTemplate.Parse "/css/main.css") Less.main
        resource (UriTemplate.Parse "/posts/{name}") Pages.post
        resource (UriTemplate.Parse "/") Pages.index
        resource (UriTemplate.Parse "/archive.html") Pages.archive
        resource (UriTemplate.Parse "/contact.html") Pages.contact
        resource (UriTemplate.Parse "/rss.xml") Rss.feed
     } |> FreyaRouter.toPipeline
