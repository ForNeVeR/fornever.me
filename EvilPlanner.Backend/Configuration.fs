namespace EvilPlanner

open System
open System.Collections.Specialized
open System.IO
open System.Reflection

type Configuration =
    { databasePath : string }
    with
        static member OfAppSettings (appSettings : NameValueCollection) : Configuration =
            let basePath =
                let absolutePath (uri : Uri) = uri.AbsolutePath
                lazy
                    Assembly.GetExecutingAssembly().CodeBase
                    |> Uri
                    |> absolutePath
                    |> Path.GetDirectoryName
            let mapPath (path : string) =
                if path.StartsWith "~"
                then Path.Combine(basePath.Value, path.Substring 2)
                else path
            { databasePath = mapPath(appSettings.["databasePath"]) }
