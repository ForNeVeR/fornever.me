namespace ForneverMind

open Microsoft.AspNetCore.NodeServices

#nowarn "0044" // TODO: Remove after dealing with NodeServices
type CodeHighlightModule(node : INodeServices) =
    member __.Highlight(language: string option, code: string): Async<string> =
        let serverModulePath = "./server"
        let lang = Option.toObj language
        async {
            let! token = Async.CancellationToken
            let! result = Async.AwaitTask <| node.InvokeAsync(token, serverModulePath, lang, code)
            return result
        }
