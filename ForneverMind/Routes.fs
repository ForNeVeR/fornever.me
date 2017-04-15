module ForneverMind.Routes

open Freya.Routers.Uri.Template

let router =
     freyaRouter {
        resource "/posts/{name}" Pages.post
        resource "/" Pages.index
        resource "/archive.html" Pages.archive
        resource "/contact.html" Pages.contact
        resource "/error.html" Pages.error
        resource "/rss.xml" Rss.feed
        resource "/talks.html" Pages.talks
        resource "/{q*}" Pages.notFound
     }
