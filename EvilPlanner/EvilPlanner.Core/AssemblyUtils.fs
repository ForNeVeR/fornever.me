// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module EvilPlanner.Core.AssemblyUtils

open System.IO
open System.Reflection

let assemblyDirectory(assembly : Assembly) : string =
    assembly.Location
    |> Path.GetDirectoryName
    |> Path.GetFullPath
