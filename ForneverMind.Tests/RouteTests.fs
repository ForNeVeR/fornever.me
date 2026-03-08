// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
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
let ``Archive page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) title = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
        Assert.Contains(title, content)
    }

    do! doTest "/en/archive.html" "Posts"
    do! doTest "/ru/archive.html" "Посты"
})

[<Fact>]
let ``Contact page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) title = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
        Assert.Contains(title, content)
    }

    do! doTest "/en/contact.html" "Contacts"
    do! doTest "/ru/contact.html" "Контакты"
})

[<Fact>]
let ``Talks page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) title = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
        Assert.Contains(title, content)
    }

    do! doTest "/en/talks.html" "Talks"
    do! doTest "/ru/talks.html" "Доклады"
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

    do! doTest "/en/404" "The requested page does not exist."
    do! doTest "/ru/404" "Страницы по этому адресу не существует."
    do! doTest "/en/blah-blah" "The requested page does not exist."
    do! doTest "/ru/blah-blah" "Страницы по этому адресу не существует."
    do! doTest "/blah-blah" "The requested page does not exist."
})
