module ForneverMind.Program

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging

open ForneverMind.KestrelInterop

let private useStaticFiles (app : IApplicationBuilder) =
    app.UseStaticFiles("/app")
        .UseStaticFiles("/images")
        .UseStaticFiles("/talks")

let private configureApplication =
    useStaticFiles >> ApplicationBuilder.useFreya Routes.router >> ignore

let private configureLogger (logger : ILoggerFactory) =
    logger.AddConsole()
    |> ignore

let configuration =
    { application = configureApplication
      logging = configureLogger }

[<EntryPoint>]
let main argv =
    WebHost.create ()
    |> WebHost.bindTo [|"http://localhost:5000"|]
    |> WebHost.configure configuration
    |> WebHost.buildAndRun

    0
