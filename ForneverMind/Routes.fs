module ForneverMind.Routes

open Arachne.Uri.Template

open Freya.Machine.Router
open Freya.Router

let router =
     freyaRouter {
        resource (UriTemplate.parse "/posts/{name}") Pages.post
        resource (UriTemplate.parse "/") Pages.index
        resource (UriTemplate.parse "/archive.html") Pages.archive
        resource (UriTemplate.parse "/contact.html") Pages.contact
        resource (UriTemplate.parse "/error.html") Pages.error
        resource (UriTemplate.parse "/rss.xml") Rss.feed
        resource (UriTemplate.parse "/{q*}") Pages.notFound
     }
