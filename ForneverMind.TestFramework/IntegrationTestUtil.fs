module ForneverMind.TestFramework.IntegrationTestUtil

open System
open System.Net.Http
open System.Threading.Tasks

open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc.Testing

open ForneverMind

[<Literal>]
let IntegrationTests = "IntegrationTests"

let private withWebAppBuilder (configure: IWebHostBuilder -> unit)
                              (customize: IServiceProvider -> 'a)
                              (test: 'a -> HttpClient -> Task): Task = task {
    use app = (new WebApplicationFactory<RoutesModule>()).WithWebHostBuilder configure
    app.Server.AllowSynchronousIO <- true

    let services = customize app.Services

    use client = app.CreateClient()
    return! test services client
}

let private withWebAppServices (customize: IServiceProvider -> 'a) (test: 'a -> HttpClient -> Task): Task =
    withWebAppBuilder ignore customize test

let withWebApp(test: HttpClient -> Task): Task = withWebAppServices ignore (fun _ -> test)
