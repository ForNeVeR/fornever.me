namespace ForneverMind.Tests

open System.Threading.Tasks

open Xunit

open ForneverMind.Tests.IntegrationTestUtil

[<Collection(IntegrationTests)>]
type RouteTests() =

    [<Fact>]
    member _.``Index page should resolve correctly``(): Task = withWebApp(fun client -> task {
        let! result = client.GetAsync "/"
        Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
    })
