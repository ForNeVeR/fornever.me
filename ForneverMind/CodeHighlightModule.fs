namespace ForneverMind

open System
open System.Diagnostics
open System.IO
open System.Threading.Tasks

open JetBrains.Collections.Viewable
open JetBrains.Lifetimes
open Jint
open Jint.Native
open Microsoft.Extensions.Logging

type JintConsole() =
    member _.Log(s: string) = Console.WriteLine s

type CodeHighlightModule(lifetime: Lifetime, logger: ILogger) =
    let engine =
        let sw = Stopwatch.StartNew()
        logger.LogInformation "Initializing Jint engine…"
        let serverCode = File.ReadAllText "server.js"
        let e = new Engine()
        lifetime.AddDispose e |> ignore

        let result =
            e
                .SetValue("console", JintConsole())
                .Execute serverCode
        logger.LogInformation $"Jint engine initialized in {sw.Elapsed}."
        result

    let server = engine.Evaluate "server"
    let scheduler = SingleThreadScheduler.RunOnSeparateThread(lifetime, "CodeHighlightModule.scheduler")

    member _.Highlight(language: string option, code: string): Async<string> = async {
        let languageName = Option.defaultValue "<none>" language
        let sw = Stopwatch.StartNew()
        logger.LogInformation $"Start processing request for {code.Length} characters in language {languageName}."

        let args = [|
            JsValue.FromObject(engine, Option.toObj language)
            JsValue.op_Implicit code
        |]

        let! ct = Async.CancellationToken
        let tcs = TaskCompletionSource<string>()
        use _ = ct.Register(tcs.TrySetCanceled >> ignore)

        scheduler.Queue(fun () ->
            try
                tcs.SetResult <| (server.Call args).AsString()
            with
                e -> tcs.TrySetException e |> ignore
        )

        let! result = Async.AwaitTask tcs.Task
        logger.LogInformation $"Finish processing request for {code.Length} characters in language {languageName}: {sw.Elapsed}."
        return result
    }
