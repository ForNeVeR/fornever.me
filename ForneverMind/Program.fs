// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Program

open System
open System.IO

open System.Threading.Tasks
open Freya.Core
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Diagnostics
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

    builder.Services.AddRazorPages() |> ignore
    builder.Services.AddMvc() |> ignore

    builder

let private build (builder: WebApplicationBuilder) =
    let app = builder.Build()
    app.UseStaticFiles() |> ignore

    // To use custom error page addresses, first apply StatusCodePagesWithReExecute and then read the original routing
    // data from IStatusCodeReExecuteFeature.
    app
        .UseStatusCodePagesWithReExecute("/error/{0}")
        .Use(fun (context: HttpContext) (next: RequestDelegate) ->
            (task {
                let statusCode = context.Response.StatusCode
                if statusCode = 404 then
                    let errorInfo = context.Features.Get<IStatusCodeReExecuteFeature>() |> ValueOption.ofObj
                    match errorInfo with
                    | ValueSome error ->
                        let language =
                            if error.OriginalPath.StartsWith "/ru/" then "ru"
                            else "en"
                        context.Response.Redirect $"/{language}/404"
                        return ()
                    | ValueNone -> return! next.Invoke context
                else
                    return! next.Invoke context
            }) : Task
    ) |> ignore

    let router = createRouter app.Services
    useFreya router app

    app.UseRouting() |> ignore
    app.MapControllers() |> ignore
    app.MapRazorPages() |> ignore
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
