namespace ForneverMind

open System
open System.Diagnostics
open System.IO
open System.Threading.Tasks

open Jint
open Jint.Native
open Microsoft.AspNetCore.NodeServices
open Microsoft.Extensions.Logging

type JintConsole() =
    member _.Log(s: string) = Console.WriteLine s

#nowarn "0044" // TODO: Remove after dealing with NodeServices
type CodeHighlightModule(logger: ILogger,  node: INodeServices) =
    member _.Highlight(language: string option, code: string): Async<string> = async {
        let languageName = Option.defaultValue "<none>" language
        let sw = Stopwatch.StartNew()
        logger.LogInformation $"Start processing request for {code.Length} characters in language {languageName}"

        let tcs = TaskCompletionSource<string>()
        let! token = Async.CancellationToken
        use _ = token.Register(fun () -> tcs.TrySetCanceled() |> ignore)

        let callback = fun error result ->
            if isNull error
            then tcs.TrySetResult(string result)
            else tcs.TrySetException(Exception(string error))
            |> ignore

        use engine =
            new Engine((*(fun opts ->
            opts.EnableModules(Environment.CurrentDirectory) |> ignore
            opts.Modules.RegisterRequire <- true*))
        engine.SetValue("console", JintConsole()) |> ignore
        let! serverCode = Async.AwaitTask(File.ReadAllTextAsync "server.js")
        engine.Execute serverCode |> ignore
        let server = engine.Evaluate "server"
        // let server = engine.ImportModule("./server.js")
        let args = [|
            JsValue.FromObject(engine, Action<obj, obj> callback)
            JsValue.FromObject(engine, "c")
            JsValue.FromObject(engine, "printf(\"Hello, world!\");")
        |]

        let console = JsValue.FromObject

        let result = server.Call args
        // engine.Execute("import server from 'server';") |> ignore
        // engine.Execute("server(setResult, 'c', '');")
        // |> ignore

        let! result = Async.AwaitTask(tcs.Task)
        logger.LogInformation $"Finish processing request for {code.Length} characters in language {languageName}: {sw.Elapsed}"
        return result
    }
