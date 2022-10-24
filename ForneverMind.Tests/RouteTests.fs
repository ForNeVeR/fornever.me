module ForneverMind.Tests.RouteTests

open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc.Testing
open Xunit

open ForneverMind.Controllers

[<Fact>]
let ``Quotes controller should resolve correctly``(): Task = task {
    use app = (new WebApplicationFactory<QuotesController>()).WithWebHostBuilder(fun _ -> ())
    use client = app.CreateClient()
    let! result = client.GetAsync("/plans/quote/123")
    let! content = result.EnsureSuccessStatusCode().Content.ReadAsStringAsync()
    Assert.Equal("123", content)
}
