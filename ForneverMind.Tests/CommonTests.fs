// SPDX-FileCopyrightText: 2017-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Tests.CommonTests

open Xunit

open ForneverMind

[<Fact>]
let ``Default language is English``() =
    Assert.Equal("en", Common.defaultLanguage)
