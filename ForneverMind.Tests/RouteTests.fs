// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

[<Xunit.Collection(ForneverMind.TestFramework.IntegrationTestUtil.IntegrationTests)>]
module ForneverMind.Tests.RouteTests

open System.Threading.Tasks

open Xunit

open ForneverMind.TestFramework.IntegrationTestUtil

[<Fact>]
let ``Index page should resolve correctly``(): Task = withWebApp(fun client -> task {
    let! result = client.GetAsync "/"
    Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
})
