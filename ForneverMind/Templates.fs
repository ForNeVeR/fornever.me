module ForneverMind.Templates

open System.IO

open RazorEngine.Configuration
open RazorEngine.Templating

let private razor =
    let templateManager = ResolvePathTemplateManager <| Seq.singleton Config.viewsDirectory
    let config = TemplateServiceConfiguration (DisableTempFileLocking = true, TemplateManager = templateManager)
    RazorEngineService.Create config

let lastModificationDate name =
    let filePath = Path.Combine (Config.viewsDirectory, name + ".cshtml")
    File.GetLastWriteTimeUtc filePath

let render<'a> (name : string) (model : 'a option) : Async<string> =
    async {
        do! Async.SwitchToThreadPool ()
        let value =
            match model with
            | Some(v) -> v
            | None -> Unchecked.defaultof<'a>
        return razor.RunCompile (name, typeof<'a>, value)
    }
