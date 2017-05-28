namespace ForneverMind

open Microsoft.AspNetCore.NodeServices

type CodeHighlightModule(node : INodeServices) =
    member __.Highlight(language: string option, code: string): Async<string> =
        let serverModulePath = "./wwwroot/app/server"
        let lang = Option.toObj language
        async {
            let! token = Async.CancellationToken
            let! result = Async.AwaitTask <| node.InvokeAsync(token, serverModulePath, lang, code)
            return result
        }
