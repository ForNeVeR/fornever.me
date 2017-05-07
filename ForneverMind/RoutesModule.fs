namespace ForneverMind

open Freya.Routers.Uri.Template

type RoutesModule(pages : PagesModule, rss : RssModule) =
    let router =
        freyaRouter {
            resource "/posts/{name}" pages.Post
            resource "/" pages.Index
            resource "/archive.html" pages.Archive
            resource "/contact.html" pages.Contact
            resource "/error.html" pages.Error
            resource "/rss.xml" rss.Feed
            resource "/talks.html" pages.Talks
            resource "/{q*}" pages.NotFound
        }

    member __.Router = router
