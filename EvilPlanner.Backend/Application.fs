namespace EvilPlanner.Backend

open System.IO
open System.Text

open Freya.Core
open Microsoft.Owin

type Application () =
    member __.Configuration () =
        OwinAppFunc.ofFreya Quotes.router

[<assembly: OwinStartup(typeof<Application>)>]
()
