namespace ForneverMind

open System
open System.IO

open RazorLight

type TemplatingModule (config: ConfigurationModule) =
    let razor = EngineFactory.CreatePhysical config.ViewsPath

    let templateName language name = sprintf "%s/%s.cshtml" language name
    let templatePath language name = Path.Combine(config.ViewsPath, language, templateName language name)
    let layoutPath language = templatePath language "_Layout"

    member __.LastModificationDate (language : string) (name : string) : DateTime =
        let templateModificationDate = File.GetLastWriteTimeUtc <| templatePath language name
        let layoutModificationDate = File.GetLastWriteTimeUtc <| layoutPath language
        max templateModificationDate layoutModificationDate

    member __.Render<'a> (language : string) (name : string) (model : 'a option) : Async<string> =
        async {
            do! Async.SwitchToThreadPool ()
            let value =
                match model with
                | Some(v) -> v
                | None -> Unchecked.defaultof<'a>
            return razor.Parse(templateName language name, value)
        }
