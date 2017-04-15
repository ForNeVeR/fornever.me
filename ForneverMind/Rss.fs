module ForneverMind.Rss

open System
open System.IO
//open System.ServiceModel.Syndication
open System.Text
open System.Xml
// TODO[F]: Rewrite this module in a portable way; ensure that resulting output preserves post ids.
//open Arachne.Http
//open Freya.Core
//open Freya.Machine
//open Freya.Machine.Extensions.Http
//
//open ForneverMind.Models
//
//let private sindicationItem (post : PostMetadata) : SyndicationItem =
//    let url = Config.baseUrl + post.Url
//    SyndicationItem(post.Title, post.Description, Uri url, url, DateTimeOffset post.Date)
//
//let private feedContent =
//    let items = Seq.map sindicationItem Posts.allPosts
//    let feed =
//        SyndicationFeed (
//            "Engineer, programmer, gentleman",
//            "Friedrich von Never: Engineer, Programmer, Gentleman.",
//            Uri Config.baseUrl,
//            items
//        )
//    let formatter = Rss20FeedFormatter feed
//    use writer = new StringWriter ()
//    use xmlWriter = new XmlTextWriter (writer)
//    formatter.WriteTo xmlWriter
//    Encoding.UTF8.GetBytes (writer.ToString ())
//
//let private lastModificationDate =
//    defaultArg (Posts.allPosts
//                |> Seq.tryHead
//                |> Option.map (fun p -> p.Date)) DateTime.UtcNow
//
//let private handleFeed _ =
//    freya {
//        return
//            {
//                Description =
//                    {
//                        Charset = Some Charset.Utf8
//                        Encodings = None
//                        MediaType = Some Common.rss
//                        Languages = None
//                    }
//                Data = feedContent
//            }
//    }
//
let inline feed _ =
    // TODO[F]: Fix that.
    failwithf "Not implemented"
    //freyaMachine {
    //    including Common.machine
    //    methodsSupported Common.methods
    //    lastModified (Common.initLastModified lastModificationDate)
    //    handleOk handleFeed
    //}
