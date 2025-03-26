// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.TestFramework.IntegrationTestUtil

open System
open System.Net.Http
open System.Threading.Tasks

open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc.Testing
open Microsoft.Extensions.DependencyInjection

open EvilPlanner.Core
open EvilPlanner.Core.Storage
open ForneverMind

[<Literal>]
let IntegrationTests = "IntegrationTests"

let private withWebAppBuilder (configure: IWebHostBuilder -> unit)
                              (customize: IServiceProvider -> 'a)
                              (test: 'a -> HttpClient -> Task): Task = task {
    use app = (new WebApplicationFactory<RoutesModule>()).WithWebHostBuilder configure
    app.Server.AllowSynchronousIO <- true

    let config = app.Services.GetRequiredService<ConfigurationModule>()
    StorageUtils.reinitializeDatabase config.EvilPlannerConfig

    let services = customize app.Services

    use client = app.CreateClient()
    return! test services client
}

let private withWebAppServices (customize: IServiceProvider -> 'a) (test: 'a -> HttpClient -> Task): Task =
    withWebAppBuilder ignore customize test

let withWebApp(test: HttpClient -> Task): Task = withWebAppServices ignore (fun _ -> test)

let withWebAppData (today: DateOnly) (test: Database -> HttpClient -> Task): Task =
    withWebAppBuilder (fun builder ->
        let clock = { new IClock with
            member _.Today() = today
        }
        builder.ConfigureServices(fun sc -> sc.AddSingleton clock |> ignore) |> ignore
    ) (fun services -> services.GetRequiredService<Database>()) test
