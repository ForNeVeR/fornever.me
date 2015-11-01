module ForneverMind.Templates

open System.IO

open RazorEngine.Configuration
open RazorEngine.Templating

let private razor =
    let templateManager = ResolvePathTemplateManager <| Seq.singleton Config.viewsDirectory
    let config = TemplateServiceConfiguration (DisableTempFileLocking = true, TemplateManager = templateManager)
    RazorEngineService.Create config

let render<'a> (name : string) (model : 'a) : Async<string> =
    async {
        do! Async.SwitchToThreadPool ()
        return razor.RunCompile (name, typeof<'a>, model)
    }
