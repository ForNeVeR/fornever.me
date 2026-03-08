// SPDX-FileCopyrightText: 2017-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Controllers

open System
open System.Text

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open WilderMinds.RssSyndication

open ForneverMind
open ForneverMind.Core

type RssController(config: ConfigurationModule, posts: IPostsProvider) =
    inherit Controller()

    let syndicationItem (post: PostMetadata) =
        let url = config.BaseUrl + post.Url
        Item(Title = post.Title,
             Body = post.Description,
             Link = Uri url,
             Permalink = url,
             PublishDate = post.Date)

    let generateFeed (posts: PostMetadata seq) =
        let feed = Feed(
            Title = "Engineer, programmer, gentleman",
            Description = "Friedrich von Never: Engineer, Programmer, Gentleman.",
            Link = Uri config.BaseUrl
        )
        for item in Seq.map syndicationItem posts do feed.Items.Add item
        feed.Serialize(SerializeOption(Encoding = Encoding.UTF8))

    member private this.ServeFeed(allPosts: PostMetadata seq): IActionResult =
        let allPosts = Seq.toArray allPosts
        let lastModified =
            allPosts
            |> Array.tryHead
            |> Option.map (fun p -> p.Date)
            |> Option.defaultValue DateTime.UtcNow
        this.Response.GetTypedHeaders().LastModified <- DateTimeOffset lastModified
        let content = generateFeed allPosts
        this.Content(content, "application/rss+xml", Encoding.UTF8)

    [<Route("{language}/rss.xml")>]
    member this.LanguageFeed(language: string): IActionResult =
        posts.AllPosts(language) |> this.ServeFeed

    [<Route("rss.xml")>]
    member this.CombinedFeed(): IActionResult =
        ["en"; "ru"]
        |> Seq.collect posts.AllPosts
        |> Seq.sortByDescending _.Date
        |> this.ServeFeed
