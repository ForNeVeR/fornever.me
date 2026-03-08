// SPDX-FileCopyrightText: 2017-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Tests.FileSystemTests

open System
open ForneverMind.Core
open TruePath
open Xunit

[<Fact>]
let ``Path checks are passed``() =
    let aaa = AbsolutePath(if OperatingSystem.IsWindows() then @"C:\aaa" else "/aaa")
    Assert.True(FileSystem.IsPathInsideDirectory(aaa, aaa / "bbb" / "ccc"))
    Assert.False(FileSystem.IsPathInsideDirectory(aaa, aaa / ".." / "bbb"))
