// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Tests.CommonTests

open System.IO

open Xunit

open ForneverMind

[<Fact>]
let ``Path checks are passed``() =
    Assert.True(Common.pathIsInsideDirectory "aaa" (Path.Combine("aaa", "bbb", "ccc")))
    Assert.False(Common.pathIsInsideDirectory "aaa" (Path.Combine("aaa", "..", "bbb")))

[<Fact>]
let ``Default language is English``() =
    Assert.Equal("en", Common.defaultLanguage)

[<Fact>]
let ``Supported languages are English and Russian``() =
    Assert.Equal<string>([| "en"; "ru" |], Common.supportedLanguages)
