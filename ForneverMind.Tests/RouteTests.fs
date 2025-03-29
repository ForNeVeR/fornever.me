// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

[<Xunit.Collection(ForneverMind.TestFramework.IntegrationTestUtil.IntegrationTests)>]
module ForneverMind.Tests.RouteTests

open System.Net
open System.Threading.Tasks

open Xunit

open ForneverMind.TestFramework.IntegrationTestUtil

[<Fact>]
let ``Index page should resolve correctly``(): Task = withWebApp(fun client -> task {
    let! result = client.GetAsync "/"
    Assert.Equal("/en/", result.RequestMessage.RequestUri.PathAndQuery)
    Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
})

[<Fact>]
let ``404 page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) message = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode)
        Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
        Assert.Contains(message, content)
    }

    do! doTest "/en/blah-blah" "The requested page does not exist."
    do! doTest "/ru/blah-blah" "Страница по этому адресу не существует."
    do! doTest "/blah-blah" "The requested page does not exist."
})
