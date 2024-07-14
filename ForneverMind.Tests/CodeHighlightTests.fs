module ForneverMind.Tests.CodeHighlightTests

open System.Threading.Tasks
open Microsoft.Extensions.Logging.Abstractions
open Xunit

open ForneverMind

[<Fact>]
let ``CodeHighlightModule should work with parallel queries``(): Task =
    let doTest(highlight: CodeHighlightModule) = task {
        let highlight() = task {
            use server = highlight.StartServer()
            return! Async.StartAsTask <| highlight.Highlight(server, Some "c", "printf();")
        }

        let tasks = [| for _ in 1..50 do highlight() |]
        let! results = Task.WhenAll tasks
        for r in results do
            Assert.Equal("<span class=\"hljs-built_in\">printf</span>();", r)
    }

    let highlight = CodeHighlightModule(NullLogger.Instance)
    doTest highlight
