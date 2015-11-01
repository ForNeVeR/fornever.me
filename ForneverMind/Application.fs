namespace ForneverMind

open Freya.Core
open Microsoft.Owin

type Application () =
    member __.Configuration () =
        OwinAppFunc.ofFreya Routes.router

[<assembly: OwinStartup(typeof<Application>)>]
()
