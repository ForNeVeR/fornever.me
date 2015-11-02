module ForneverMind.Markdown

let render fileName =
    async {
        do! Async.SwitchToThreadPool ()
        return "" // TODO: Render the passed file
    }
