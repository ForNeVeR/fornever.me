module ForneverMind.Program

open ForneverMind.KestrelInterop

[<EntryPoint>]
let main argv =
    let configureApp =
        ApplicationBuilder.useFreya Routes.router
        ApplicationBuilder.useStaticFiles

    WebHost.create ()
    |> WebHost.bindTo [|"http://localhost:5000"|]
    |> WebHost.configure configureApp
    |> WebHost.buildAndRun

    0
