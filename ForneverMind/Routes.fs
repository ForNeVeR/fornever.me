module ForneverMind.Routes

open Arachne.Uri.Template

open Freya.Machine.Router
open Freya.Router

let router =
    freyaRouter {
       resource (UriTemplate.Parse "/css/main.css") Less.main
       // TODO: Add language-less URL handling here. ~ F
       resource (UriTemplate.Parse "/{language}/posts/{name}") Pages.post
       resource (UriTemplate.Parse "/{language}/") Pages.index
       resource (UriTemplate.Parse "/{language}/archive.html") Pages.archive
       resource (UriTemplate.Parse "/{language}/contact.html") Pages.contact
       resource (UriTemplate.Parse "/{language}/rss.xml") Rss.feed
    } |> FreyaRouter.toPipeline
