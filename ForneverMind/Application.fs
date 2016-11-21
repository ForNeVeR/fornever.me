namespace ForneverMind

open Freya.Core
open Microsoft.Owin
open Owin

type Application () =
    member __.Configuration (app : IAppBuilder) =
        ignore<| app.UseStaticFiles("/app")
            .UseStaticFiles("/images")
            .UseStaticFiles("/talks")
        let router = OwinAppFunc.ofFreya Routes.router
        app.Run (fun c -> router.Invoke c.Environment)

[<assembly: OwinStartup(typeof<Application>)>]
()
