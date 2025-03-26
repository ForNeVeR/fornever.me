// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System
open System.Diagnostics
open System.IO

open Jint
open Jint.Native
open Microsoft.Extensions.Logging

type JintConsole(logger: ILogger) =
    member _.Log(s: string) = logger.LogInformation s

type Server =
    | Server of Engine * JsValue
    interface IDisposable with
        member this.Dispose() =
            let (Server(engine, _)) = this
            engine.Dispose()

type CodeHighlightModule(logger: ILogger) =
    let createEngine(): Engine =
        let sw = Stopwatch.StartNew()
        logger.LogInformation "Initializing Jint engine…"
        let serverCode = File.ReadAllText "server.js"
        let e = new Engine()

        let result =
            e
                .SetValue("console", JintConsole logger)
                .Execute serverCode
        logger.LogInformation $"Jint engine initialized in {sw.Elapsed}."
        result

    member _.StartServer(): Server =
        let engine = createEngine()
        Server(engine, engine.Evaluate "server")

    member _.Highlight(Server(engine, server), language: string option, code: string): Async<string> = async {
        let sw = Stopwatch.StartNew()
        let languageName = Option.defaultValue "<none>" language
        logger.LogInformation $"Start processing request for {code.Length} characters in language {languageName}."

        let args = [|
            JsValue.FromObject(engine, Option.toObj language)
            JsValue.op_Implicit code
        |]
        let result = server.Call(args).AsString()

        logger.LogInformation $"Finish processing request for {code.Length} characters in language {languageName}: {sw.Elapsed}."
        return result
    }
