module ForneverMind.Templates

open System.IO

open RazorEngine
open RazorEngine.Templating

let private readFile path =
    async {
        use stream = File.OpenText path
        return! Async.AwaitTask <| stream.ReadToEndAsync ()
    }

let private getViewPath name =
    let path = Path.Combine (Common.viewsDirectory, name + ".cshtml")
    assert (Path.GetDirectoryName path = Common.viewsDirectory)
    path

let execute<'a> name (model : 'a) =
    async {
        let path = getViewPath name
        let! template = readFile path

        return (Engine.Razor.RunCompile (template, name, typeof<'a>, model))
    }
