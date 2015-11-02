module ForneverMind.Markdown

open System.IO
open System.Text

let render (fileName : string) =
    async {
        do! Async.SwitchToThreadPool ()
        use stream = new StreamReader (fileName, Encoding.UTF8)
        use target = new StringWriter ()
        CommonMark.CommonMarkConverter.Convert (stream, target)
        return target.ToString ()
    }
