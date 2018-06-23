module EvilPlanner.Core.AssemblyUtils

open System
open System.IO
open System.Reflection

let assemblyDirectory(assembly : Assembly) : string =
    let absolutePath(uri : Uri) = uri.AbsolutePath
    Assembly.GetExecutingAssembly().Location
    |> Uri
    |> absolutePath
    |> Path.GetDirectoryName
    |> Path.GetFullPath
