module ForneverMind.Tests.IntegrationTestUtil

open System
open System.Net.Http
open System.Threading.Tasks

open Microsoft.AspNetCore.Mvc.Testing
open Microsoft.Extensions.DependencyInjection

open EvilPlanner.Core.Storage
open ForneverMind

[<Literal>]
let IntegrationTests = "IntegrationTests"

let private withWebAppServices (customize: IServiceProvider -> 'a) (test: 'a -> HttpClient -> Task): Task = task {
    // TODO: Mock the database to have a separate data directory for each test.
    use app = new WebApplicationFactory<RoutesModule>()
    app.Server.AllowSynchronousIO <- true

    let services = customize app.Services

    use client = app.CreateClient()
    return! test services client
}

let withWebApp(test: HttpClient -> Task): Task = withWebAppServices ignore (fun _ -> test)

let withWebAppData(test: Database -> HttpClient -> Task): Task =
    withWebAppServices (fun services -> services.GetRequiredService<Database>()) test
