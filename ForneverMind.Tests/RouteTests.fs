// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

[<Xunit.Collection(ForneverMind.TestFramework.IntegrationTestUtil.IntegrationTests)>]
module ForneverMind.Tests.RouteTests

open System.Net
open System.Threading.Tasks
open System.Xml.Linq

open Xunit

open ForneverMind.TestFramework.IntegrationTestUtil

[<Fact>]
let ``Index page should resolve correctly``(): Task = withWebApp(fun client -> task {
    let! result = client.GetAsync "/"
    Assert.Equal("/en/", (nonNull (nonNull result.RequestMessage).RequestUri).PathAndQuery)
    Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
})

[<Fact>]
let ``Index page should have correct titles``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
        Assert.Contains("int 20h", content)
        Assert.True(result.Content.Headers.LastModified.HasValue)
    }

    do! doTest "/en/"
    do! doTest "/ru/"
})

[<Fact>]
let ``Archive page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) title = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
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
        Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
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
        Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
        Assert.Contains(title, content)
    }

    do! doTest "/en/talks.html" "Talks"
    do! doTest "/ru/talks.html" "Доклады"
})

[<Fact>]
let ``Post page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) title = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
        Assert.Contains(title, content)
    }

    // This post exists in both languages
    do! doTest "/en/posts/2017-11-07-multilingual-support.html" "Multilingual support"
    do! doTest "/ru/posts/2017-11-07-multilingual-support.html" "Поддержка нескольких языков"
})

[<Fact>]
let ``Non-existent post should return 404``(): Task = withWebApp(fun client -> task {
    let! result = client.GetAsync "/en/posts/9999-01-01-does-not-exist.html"
    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode)
})

[<Fact>]
let ``RSS feeds should return valid XML with items``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        Assert.Equal("application/rss+xml", (nonNull result.Content.Headers.ContentType).MediaType)

        Assert.True(result.Content.Headers.LastModified.HasValue)

        let doc = XDocument.Parse content
        let items = doc.Descendants(XName.Get "item")
        Assert.NotEmpty items
    }

    do! doTest "/en/rss.xml"
    do! doTest "/ru/rss.xml"
    do! doTest "/rss.xml"
})

[<Fact>]
let ``404 page should be resolved``(): Task = withWebApp(fun client -> task {
    let doTest (url: string) message = task {
        let! result = client.GetAsync url
        let! content = result.Content.ReadAsStringAsync()

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode)
        Assert.Equal("text/html", (nonNull result.Content.Headers.ContentType).MediaType)
        Assert.Contains(message, content)
    }

    do! doTest "/en/404" "The requested page does not exist."
    do! doTest "/ru/404" "Страницы по этому адресу не существует."
    do! doTest "/en/blah-blah" "The requested page does not exist."
    do! doTest "/ru/blah-blah" "Страницы по этому адресу не существует."
    do! doTest "/blah-blah" "The requested page does not exist."
})
