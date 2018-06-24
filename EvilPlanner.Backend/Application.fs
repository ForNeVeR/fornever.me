module EvilPlanner.Backend.Application

open System
open System.Configuration
open System.IO
open System.Reflection

open Freya.Core
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging

open EvilPlanner.Core

let private getConfig directory =
    let configRoot =
        ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json")
            .Build()
    let basePath = lazy AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
    let mapPath (path : string) =
        if path.StartsWith "~"
        then Path.Combine(basePath.Value, path.Substring 2)
        else path
    { databasePath = mapPath(configRoot.["databasePath"]) }

let private application config (app : IApplicationBuilder) =
    let lifetime = app.ApplicationServices.GetService(typeof<IApplicationLifetime>) :?> IApplicationLifetime
    let database = Storage.openDatabase config
    let atExit() = (database :> IDisposable).Dispose()
    ignore <| lifetime.ApplicationStopped.Register(Action atExit)

    let func = OwinMidFunc.ofFreya(Quotes.router database)
    ignore <| app.UseOwin(fun p -> p.Invoke func)

[<EntryPoint>]
let main (args : string[]) : int =
    let config = getConfig(Directory.GetCurrentDirectory())
    Migrations.migrateDatabase config

    let root = Directory.GetCurrentDirectory()
    let wwwRoot = Path.Combine(root, "wwwroot")
    WebHostBuilder()
        .UseContentRoot(root)
        .UseWebRoot(wwwRoot)
        .UseKestrel()
        .ConfigureLogging(fun logger -> logger.AddConsole() |> ignore)
        .Configure(Action<_>(application config))
        .Build()
        .Run()

    0
