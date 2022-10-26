[<Xunit.Collection(ForneverMind.Tests.IntegrationTestUtil.IntegrationTests)>]
module ForneverMind.Tests.RouteTests

open System.Threading.Tasks

open Xunit

open ForneverMind.Tests.IntegrationTestUtil

[<Fact>]
let ``Index page should resolve correctly``(): Task = withWebApp(fun client -> task {
    let! result = client.GetAsync "/"
    Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
})
