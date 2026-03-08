// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Controllers

open System
open System.Text

open Microsoft.AspNetCore.Mvc
open WilderMinds.RssSyndication

open ForneverMind
open ForneverMind.Core

[<Route("")>]
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

    [<Route("{language}/rss.xml")>]
    member this.LanguageFeed(language: string): IActionResult =
        let content = posts.AllPosts(language) |> generateFeed
        this.Content(content, "application/rss+xml", Encoding.UTF8)

    [<Route("rss.xml")>]
    member this.CombinedFeed(): IActionResult =
        let content = ["en"; "ru"] |> Seq.collect posts.AllPosts |> generateFeed
        this.Content(content, "application/rss+xml", Encoding.UTF8)
