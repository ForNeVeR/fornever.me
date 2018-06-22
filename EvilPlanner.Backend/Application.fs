namespace EvilPlanner.Backend

open System.Configuration
open System.IO
open System.Text

open Freya.Core
open Microsoft.Owin

open EvilPlanner
open EvilPlanner.Data

type Application () =
    member __.Configuration () =
        let config = Configuration.OfAppSettings ConfigurationManager.AppSettings
        Async.RunSynchronously <| Storage.migrateDatabase config
        OwinAppFunc.ofFreya Quotes.router

[<assembly: OwinStartup(typeof<Application>)>]
()
