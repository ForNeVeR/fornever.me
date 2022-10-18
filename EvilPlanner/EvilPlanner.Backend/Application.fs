module EvilPlanner.Backend.Application

open System
open System.IO
open System.Reflection

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

open EvilPlanner.Core
open EvilPlanner.Core.Storage

let getConfig(databasePath: string): Configuration =
    let basePath = lazy AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
    let mapPath (path : string) =
        if path.StartsWith "~"
        then Path.Combine(basePath.Value, path.Substring 2)
        else path
    { databasePath = mapPath databasePath }

let initDatabase(config: Configuration) (app: IApplicationBuilder): Database =
    Migrations.migrateDatabase config
    let lifetime = app.ApplicationServices.GetService(typeof<IHostApplicationLifetime>) :?> IHostApplicationLifetime
    let database = openDatabase config
    let atExit() = (database :> IDisposable).Dispose()
    ignore <| lifetime.ApplicationStopped.Register(Action atExit)
    database
