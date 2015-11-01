module ForneverMind.Templates

open System.IO

open RazorEngine.Configuration
open RazorEngine.Templating

let private razor =
    let templateManager = ResolvePathTemplateManager <| Seq.singleton Config.viewsDirectory
    let config = TemplateServiceConfiguration (DisableTempFileLocking = true, TemplateManager = templateManager)
    RazorEngineService.Create config

let render<'a when 'a : null> (name : string) (model : 'a option) : Async<string> =
    async {
        do! Async.SwitchToThreadPool ()
        return razor.RunCompile (name, typeof<'a>, Option.toObj model)
    }
