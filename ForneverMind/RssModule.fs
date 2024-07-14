namespace ForneverMind

open System
open System.Text

open WilderMinds.RssSyndication
open Freya.Core
open Freya.Machines.Http
open Freya.Types.Http

open ForneverMind.Models

type RssModule(config : ConfigurationModule, posts : PostsModule) =

    let syndicationItem(post : PostMetadata): Item =
        let url = config.BaseUrl + post.Url
        Item(Title = post.Title,
             Body = post.Description,
             Link = Uri url,
             Permalink = url,
             PublishDate = post.Date)

    let feedContent posts =
        let items = Seq.map syndicationItem posts
        let feed = Feed (
            Title = "Engineer, programmer, gentleman",
            Description = "Friedrich von Never: Engineer, Programmer, Gentleman.",
            Link = Uri config.BaseUrl
        )
        Seq.iter feed.Items.Add items
        let text = feed.Serialize(SerializeOption(Encoding = Encoding.UTF8))
        Encoding.UTF8.GetBytes text

    let getPosts =
        freya {
            let! languageOpt = Common.routeLanguageOpt
            let posts =
                languageOpt
                |> Option.map Seq.singleton
                |> Option.defaultValue (Seq.ofArray Common.supportedLanguages)
                |> Seq.collect posts.AllPosts
            return posts
        }

    let lastModificationDateTime =
        freya {
            let! posts = getPosts
            return defaultArg (posts
                               |> Seq.tryHead
                               |> Option.map (fun p -> p.Date)) DateTime.UtcNow
        }

    let handleFeed =
        freya {
            let! posts = getPosts
            let content = feedContent posts
            return
                {
                    Description =
                        {
                            Charset = Some Charset.Utf8
                            Encodings = None
                            MediaType = Some Common.rss
                            Languages = None
                        }
                    Data = content
                }
        }

    member __.Feed =
        freyaMachine {
           including Common.machine
           methods Common.methods
           lastModified lastModificationDateTime
           handleOk handleFeed
        }
