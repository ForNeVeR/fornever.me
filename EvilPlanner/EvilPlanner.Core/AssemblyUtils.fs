module EvilPlanner.Core.AssemblyUtils

open System
open System.IO
open System.Reflection

let assemblyDirectory(assembly : Assembly) : string =
    let absolutePath(uri : Uri) = uri.AbsolutePath
    assembly.CodeBase
    |> Uri
    |> absolutePath
    |> Path.GetDirectoryName
    |> Path.GetFullPath
