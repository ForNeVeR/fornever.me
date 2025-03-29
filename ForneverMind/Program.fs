// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Program

open System
open System.IO

open System.Threading.Tasks
open Freya.Core
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

let private fuseApplication (services: IServiceProvider) =
    let configuration = services.GetRequiredService<ConfigurationModule>()
    let logger = services.GetRequiredService<ILogger<CodeHighlightModule>>()

    let highlight = CodeHighlightModule(logger)
    let markdown = MarkdownModule(highlight)
    let posts = PostsModule(configuration, markdown)
    let rss = RssModule(configuration, posts)
    let templates = TemplatingModule(configuration)
    let pages = PagesModule(posts, templates, markdown)

    RoutesModule(pages, rss)

let private createRouter services =
    let routesModule = fuseApplication services
    routesModule.Router

let inline private useFreya f (app: IApplicationBuilder) =
    let owin = OwinMidFunc.ofFreya f
    app.UseOwin(fun p -> p.Invoke owin)
    |> ignore

let private configure (configuration: IConfigurationRoot) (builder: WebApplicationBuilder) =
    builder.Logging.AddConsole() |> ignore
    builder.WebHost.ConfigureKestrel(fun opts -> opts.AllowSynchronousIO <- true) |> ignore

    let configModule = ConfigurationModule(builder.Environment, configuration)

    builder.Services
        .AddSingleton(configModule)
    |> ignore

    builder.Services.AddMvc() |> ignore

    builder

let private build (builder: WebApplicationBuilder) =
    let app = builder.Build()
    app.UseStaticFiles() |> ignore
    let router = createRouter app.Services
    useFreya router app
    app.UseRouting() |> ignore

    app.Use(fun (context: HttpContext) (next: RequestDelegate) ->
                (task {
                    // TODO: Debug purpose only. Remove this.
                    Console.WriteLine  $"Found: {context.GetEndpoint().DisplayName}"
                    return! next.Invoke context
                }) : Task
    ) |> ignore

    app.MapControllers() |> ignore
    app

let private run(app: WebApplication) =
    app.Run()

[<EntryPoint>]
let main(args: string[]): int =
    let cfg =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()

    WebApplication.CreateBuilder(args)
    |> configure cfg
    |> build
    |> run

    0
