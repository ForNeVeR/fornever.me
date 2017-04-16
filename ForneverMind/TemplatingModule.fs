namespace ForneverMind

open System
open System.IO

open RazorLight

type TemplatingModule (config: ConfigurationModule) =
    // TODO[F]: Fix weird stuff regarding PreserveCompilationContext
    let razor = EngineFactory.CreatePhysical config.ViewsPath

    let templateName name = name + ".cshtml"
    let templatePath name = Path.Combine(config.ViewsPath, templateName name)
    let layoutPath = templatePath "_Layout"

    member __.LastModificationDate (name : string) : DateTime =
        let templateModificationDate = File.GetLastWriteTimeUtc <| templatePath name
        let layoutModificationDate = File.GetLastWriteTimeUtc <| layoutPath
        max templateModificationDate layoutModificationDate

    member __.Render<'a> (name : string) (model : 'a option) : Async<string> =
        async {
            do! Async.SwitchToThreadPool ()
            let value =
                match model with
                | Some(v) -> v
                | None -> Unchecked.defaultof<'a>
            return razor.Parse(templateName name, value)
        }
