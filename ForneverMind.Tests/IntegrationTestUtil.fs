module ForneverMind.Tests.IntegrationTestUtil

open System.Net.Http
open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc.Testing

open ForneverMind

let withWebApp(test: HttpClient -> Task): Task = task {
    use app = new WebApplicationFactory<RoutesModule>()
    app.Server.AllowSynchronousIO <- true

    use client = app.CreateClient()
    return! test client
}
