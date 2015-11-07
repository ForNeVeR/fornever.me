module ForneverMind.Markdown

open System
open System.IO
open System.Text

open CommonMark

open ForneverMind.Models

let private processMetadata _ =
    {
        Title = ""
        CommentThreadId = ""
        DateTime = DateTime.MinValue
        HtmlContent = ""
    }

let processReader (reader : TextReader)  =
    use target = new StringWriter ()
    let document = CommonMarkConverter.Parse reader
    let post = processMetadata <| document.AsEnumerable ()

    CommonMarkConverter.ProcessStage3 (document, target)

    { post with HtmlContent = target.ToString () }


let render (fileName : string): Async<PostModel> =
    async {
        do! Async.SwitchToThreadPool ()

        use reader = new StreamReader (fileName, Encoding.UTF8)
        return processReader reader
    }
