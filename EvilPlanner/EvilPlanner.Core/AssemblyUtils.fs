module EvilPlanner.Core.AssemblyUtils

open System.IO
open System.Reflection

let assemblyDirectory(assembly : Assembly) : string =
    assembly.Location
    |> Path.GetDirectoryName
    |> Path.GetFullPath
