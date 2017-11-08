namespace ForneverMind

open System
open System.Collections.Generic
open System.IO

open RazorLight

type LanguageLink =
    { IsActive : bool
      Link : string }

type LanguageLinks =
    { English : LanguageLink
      Russian : LanguageLink }

type TemplatingModule (config: ConfigurationModule) =
    let razor = EngineFactory.CreatePhysical config.ViewsPath

    let templateName language name = sprintf "%s/%s.cshtml" language name
    let templatePath language name = Path.Combine(config.ViewsPath, templateName language name)
    let layoutPath language = templatePath language "_Layout"

    let prepareViewBag (links : LanguageLinks) =
        let viewBag = Dynamic.ExpandoObject()
        let dictionary = viewBag :> IDictionary<string, obj>
        dictionary.["Links"] <- links
        viewBag

    member __.LastModificationDate (language : string) (name : string) : DateTime =
        let templateModificationDate = File.GetLastWriteTimeUtc <| templatePath language name
        let layoutModificationDate = File.GetLastWriteTimeUtc <| layoutPath language
        max templateModificationDate layoutModificationDate

    member __.Render<'a> (language : string)
                         (name : string)
                         (model : 'a option)
                         (links : LanguageLinks) : Async<string> =
        async {
            do! Async.SwitchToThreadPool ()
            let value =
                match model with
                | Some(v) -> v
                | None -> Unchecked.defaultof<'a>
            let viewBag = prepareViewBag links
            return razor.Parse(templateName language name, value, viewBag)
        }
