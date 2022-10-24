module ForneverMind.Tests.RouteTests

open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc.Testing
open Xunit

open ForneverMind

[<Fact>]
let ``Index page should resolve correctly``(): Task = task {
    use app = new WebApplicationFactory<RoutesModule>()
    app.Server.AllowSynchronousIO <- true

    use client = app.CreateClient()
    let! result = client.GetAsync "/"
    Assert.Equal("text/html", result.Content.Headers.ContentType.MediaType)
}
