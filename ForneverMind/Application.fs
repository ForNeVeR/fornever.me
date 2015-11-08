namespace ForneverMind

open Freya.Core
open Microsoft.Owin
open Owin

type Application () =
    member __.Configuration (app : IAppBuilder) =
        app.UseStaticFiles "/images" |> ignore
        app.UseStaticFiles "/fonts" |> ignore
        let router = OwinAppFunc.ofFreya Routes.router
        app.Run (fun c -> router.Invoke c.Environment)

[<assembly: OwinStartup(typeof<Application>)>]
()
