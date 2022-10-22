module ForneverMind.Tests.CodeHighlightTests

open JetBrains.Lifetimes
open Microsoft.Extensions.Logging.Abstractions
open Xunit

open ForneverMind

[<Fact>]
let ``CodeHighlightModule should work with parallel queries``(): unit =
    let doTest(highlight: CodeHighlightModule) = async {
        let tasks = [| for _ in 1..50 do highlight.Highlight(Some "c", "printf();") |]
        let! results = Async.Parallel tasks
        for r in results do
            Assert.Equal("<span class=\"hljs-built_in\">printf</span>();", r)
    }

    Lifetime.Using(fun lt ->
        let highlight = CodeHighlightModule(lt, NullLogger.Instance)
        Async.RunSynchronously <| doTest highlight
    )
