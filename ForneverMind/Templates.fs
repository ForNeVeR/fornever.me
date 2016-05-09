module ForneverMind.Templates

open System.IO

open RazorEngine.Configuration
open RazorEngine.Templating

let private createRazor language =
    let path = Path.Combine (Config.viewsDirectory, language)
    let templateManager = ResolvePathTemplateManager <| Seq.singleton path
    let config = TemplateServiceConfiguration (DisableTempFileLocking = true, TemplateManager = templateManager)
    language, RazorEngineService.Create config

let private razorInstances = Map.ofList [ createRazor "en"
                                          createRazor "ru" ]

let lastModificationDate language name =
    let filePath = Path.Combine (Config.viewsDirectory, language, name + ".cshtml")
    File.GetLastWriteTimeUtc filePath

let render<'a> (language : string) (name : string) (model : 'a option) : Async<string> =
    async {
        do! Async.SwitchToThreadPool ()
        let value =
            match model with
            | Some(v) -> v
            | None -> Unchecked.defaultof<'a>
        let razor = Map.find language razorInstances
        return razor.RunCompile (name, typeof<'a>, value)
    }
