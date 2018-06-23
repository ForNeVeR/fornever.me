namespace EvilPlanner.Backend

open System
open System.Configuration
open System.IO
open System.Reflection
open System.Text

open Freya.Core
open Microsoft.Owin

open EvilPlanner
open EvilPlanner.Core

type Application () =
    let config =
        let basePath = lazy AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
        let mapPath (path : string) =
            if path.StartsWith "~"
            then Path.Combine(basePath.Value, path.Substring 2)
            else path
        { databasePath = mapPath(ConfigurationManager.AppSettings.["databasePath"]) }

    member __.Configuration () =
        Migrations.migrateDatabase config
        OwinAppFunc.ofFreya(Quotes.router config)

[<assembly: OwinStartup(typeof<Application>)>]
()
