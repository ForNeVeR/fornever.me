namespace EvilPlanner.Backend

open System.IO
open System.Text

open Freya.Core
open Microsoft.Owin

open EvilPlanner.Data

type Application () =
    member __.Configuration () =
        Migrations.Configuration.EnableAutoMigration ()
        OwinAppFunc.ofFreya Quotes.router

[<assembly: OwinStartup(typeof<Application>)>]
()
