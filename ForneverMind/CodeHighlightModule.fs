namespace ForneverMind

open System.Diagnostics
open Microsoft.AspNetCore.NodeServices
open Microsoft.Extensions.Logging

#nowarn "0044" // TODO: Remove after dealing with NodeServices
type CodeHighlightModule(logger: ILogger,  node: INodeServices) =
    member __.Highlight(language: string option, code: string): Async<string> =
        let languageName = Option.defaultValue "<none>" language
        let sw = Stopwatch.StartNew()
        logger.LogInformation $"Start processing request for {code.Length} characters in language {languageName}"
        let serverModulePath = "./server"
        let lang = Option.toObj language
        async {
            let! token = Async.CancellationToken
            let! result = Async.AwaitTask <| node.InvokeAsync(token, serverModulePath, lang, code)
            logger.LogInformation $"Finish processing request for {code.Length} characters in language {languageName}: {sw.Elapsed}"
            return result
        }
