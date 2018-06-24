namespace EvilPlanner.Backend

open System
open System.Configuration
open System.IO
open System.Reflection

open Freya.Core
open Microsoft.Owin
open Microsoft.Owin.BuilderProperties
open Owin

open EvilPlanner.Core

type Application() =
    let config =
        let basePath = lazy AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
        let mapPath (path : string) =
            if path.StartsWith "~"
            then Path.Combine(basePath.Value, path.Substring 2)
            else path
        { databasePath = mapPath(ConfigurationManager.AppSettings.["databasePath"]) }
    do Migrations.migrateDatabase config

    member __.Configuration(app : IAppBuilder) : unit =
        let db = Storage.openDatabase config
        let func = OwinMidFunc.ofFreya(Quotes.router db)
        let appProperties = AppProperties app.Properties

        ignore <| appProperties.OnAppDisposing.Register(fun _ -> (db :> IDisposable).Dispose())
        ignore <| app.Use func

[<assembly: OwinStartup(typeof<Application>)>]
()
