module EvilPlanner.Backend.Application

open System.IO
open System.Reflection

open JetBrains.Lifetimes

open EvilPlanner.Core
open EvilPlanner.Core.Storage

let getConfig(databasePath: string): Configuration =
    let basePath = lazy AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
    let mapPath (path : string) =
        if path.StartsWith "~"
        then Path.Combine(basePath.Value, path.Substring 2)
        else path
    { databasePath = mapPath databasePath }

let initDatabase (lifetime: Lifetime) (config: Configuration): Database =
    Migrations.migrateDatabase config
    let database = openDatabase config
    lifetime.AddDispose database |> ignore
    database
