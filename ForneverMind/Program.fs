// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Program

open System
open System.IO

open System.Threading.Tasks
open ForneverMind.Core
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Diagnostics
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open System.Text.Unicode
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.WebEncoders

let private configure (configuration: IConfigurationRoot) (builder: WebApplicationBuilder) =
    builder.Logging.AddConsole() |> ignore
    builder.WebHost.ConfigureKestrel(fun opts -> opts.AllowSynchronousIO <- true) |> ignore

    let configModule = ConfigurationModule(builder.Environment, configuration)

    builder.Services
        .AddSingleton(configModule)
        .AddSingleton<CodeHighlightModule>()
        .AddSingleton<MarkdownModule>()
        .AddSingleton<IPostsProvider, PostsProvider>()
        .AddSingleton<IPostRenderer, PostRenderer>()
    |> ignore

    // Avoid mangling Cyrillic characters in raw HTML:
    builder.Services.Configure<WebEncoderOptions>(fun (opts: WebEncoderOptions) ->
        opts.TextEncoderSettings <- Text.Encodings.Web.TextEncoderSettings(UnicodeRanges.All)
    ) |> ignore

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

    app.UseRouting() |> ignore
    app.MapControllers() |> ignore
    app.MapRazorPages() |> ignore
    app

let private run(app: WebApplication) =
    app.Run()

type internal IntegrationTestMarker = class end

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
