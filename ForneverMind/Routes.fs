module ForneverMind.Routes

open Arachne.Uri.Template

open Freya.Machine.Router
open Freya.Router

let router =
     freyaRouter {
        resource (UriTemplate.Parse "/css/main.css") Less.main
        resource (UriTemplate.Parse "/posts/{name}") Pages.post
        resource (UriTemplate.Parse "/") Pages.index
        resource (UriTemplate.Parse "/404.html") Pages.notFound
        resource (UriTemplate.Parse "/error.html") Pages.error
        resource (UriTemplate.Parse "/archive.html") Pages.archive
        resource (UriTemplate.Parse "/contact.html") Pages.contact
        resource (UriTemplate.Parse "/rss.xml") Rss.feed
     } |> FreyaRouter.toPipeline
