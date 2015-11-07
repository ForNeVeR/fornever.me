module ForneverMind.Templates

open RazorEngine.Configuration
open RazorEngine.Templating

let private razor =
    let templateManager = ResolvePathTemplateManager <| Seq.singleton Config.viewsDirectory
    let config = TemplateServiceConfiguration (DisableTempFileLocking = true, TemplateManager = templateManager)
    RazorEngineService.Create config

let render<'a> (name : string) (model : 'a option) : Async<string> =
    async {
        do! Async.SwitchToThreadPool ()
        let value =
            match model with
            | Some(v) -> v
            | None -> Unchecked.defaultof<'a>
        return razor.RunCompile (name, typeof<'a>, value)
    }
