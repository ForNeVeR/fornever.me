namespace ForneverMind

open System
open System.Text

open WilderMinds.RssSyndication
open Freya.Core
open Freya.Machines.Http
open Freya.Types.Http

open ForneverMind.Models

type RssModule(config : ConfigurationModule, posts : PostsModule) =

    let sindicationItem (post : PostMetadata) : Item =
        let url = config.BaseUrl + post.Url
        Item(Title = post.Title,
             Body = post.Description,
             Link = Uri url,
             Permalink = url,
             PublishDate = post.Date)

    let feedContent language =
        let items = posts.AllPosts language |> Seq.map sindicationItem
        let feed =
            Feed (
                Title = "Engineer, programmer, gentleman",
                Description = "Friedrich von Never: Engineer, Programmer, Gentleman.",
                Link = Uri config.BaseUrl
            )
        Seq.iter feed.Items.Add items
        let text = feed.Serialize()
        Encoding.UTF8.GetBytes text

    let lastModificationDateTime =
        freya {
            let! language = Common.routeLanguage
            return defaultArg (posts.AllPosts language
                               |> Seq.tryHead
                               |> Option.map (fun p -> p.Date)) DateTime.UtcNow
        }

    let handleFeed _ =
        freya {
            let! language = Common.routeLanguage
            let content = feedContent language
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
