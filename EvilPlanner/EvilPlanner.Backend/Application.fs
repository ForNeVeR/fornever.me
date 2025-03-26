// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module EvilPlanner.Backend.Application

open System.IO
open System.Reflection

open EvilPlanner.Core

let getConfig(databasePath: string): Configuration =
    let basePath = lazy AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
    let mapPath (path : string) =
        if path.StartsWith "~"
        then Path.Combine(basePath.Value, path.Substring 2)
        else path
    { databasePath = mapPath databasePath }
