module ForneverMind.Rss

open System
open System.IO
open System.ServiceModel.Syndication
open System.Text
open System.Xml

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http

open ForneverMind.Models

let private sindicationItem (post : PostMetadata) : SyndicationItem =
    let url = Config.baseUrl + post.Url
    SyndicationItem(post.Title, post.Description, Uri url, url, DateTimeOffset post.Date)

let private feedContent language =
    let items = Posts.allPosts language |> Seq.map sindicationItem
    let feed =
        SyndicationFeed (
            "Engineer, programmer, gentleman",
            "Friedrich von Never: Engineer, Programmer, Gentleman.",
            Uri Config.baseUrl,
            items
        )
    let formatter = Rss20FeedFormatter feed
    use writer = new StringWriter ()
    use xmlWriter = new XmlTextWriter (writer)
    formatter.WriteTo xmlWriter
    Encoding.UTF8.GetBytes (writer.ToString ())

let private lastModificationDateTime =
    freya {
        let! language = Common.routeLanguage
        return defaultArg (Posts.allPosts language
                           |> Seq.tryHead
                           |> Option.map (fun p -> p.Date)) DateTime.UtcNow
    }

let private handleFeed _ =
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

let feed =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        lastModified lastModificationDateTime
        handleOk handleFeed
    } |> FreyaMachine.toPipeline
