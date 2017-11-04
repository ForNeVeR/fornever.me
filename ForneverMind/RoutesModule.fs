namespace ForneverMind

open Freya.Routers.Uri.Template

type RoutesModule(pages : PagesModule, rss : RssModule) =
    let router =
        freyaRouter {
            // TODO: Add language-less URL handling here. ~ F
            resource "/{language}/posts/{name}" pages.Post
            resource "/{language}/" pages.Index
            resource "/{language}/archive.html" pages.Archive
            resource "/{language}/contact.html" pages.Contact
            resource "/{language}/error.html" pages.Error
            resource "/{language}/rss.xml" rss.Feed
            resource "/{language}/talks.html" pages.Talks
            resource "/{q*}" pages.NotFound
        }

    member __.Router = router
