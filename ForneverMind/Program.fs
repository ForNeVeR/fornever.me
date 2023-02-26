module ForneverMind.Program

open System
open System.IO

open Freya.Core
open JetBrains.Lifetimes
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open EvilPlanner.Core
open EvilPlanner.Core.Storage

let private fuseApplication lifetime (services: IServiceProvider) =
    let configuration = services.GetRequiredService<ConfigurationModule>()
    let logger = services.GetRequiredService<ILogger<CodeHighlightModule>>()

    let highlight = CodeHighlightModule(lifetime, logger)
    let markdown = MarkdownModule(highlight)
    let posts = PostsModule(configuration, markdown)
    let rss = RssModule(configuration, posts)
    let templates = TemplatingModule(configuration)
    let pages = PagesModule(posts, templates, markdown)

    RoutesModule(pages, rss)

let private createRouter lifetime services =
    let routesModule = fuseApplication lifetime services
    routesModule.Router

let inline private useFreya f (app: IApplicationBuilder) =
    let owin = OwinMidFunc.ofFreya f
    app.UseOwin(fun p -> p.Invoke owin)
    |> ignore

let private configure (lifetime: Lifetime) (configuration: IConfigurationRoot) (builder: WebApplicationBuilder) =
    builder.Logging.AddConsole() |> ignore
    builder.WebHost.ConfigureKestrel(fun opts -> opts.AllowSynchronousIO <- true) |> ignore

    let configModule = ConfigurationModule(builder.Environment, configuration)
    let clock = SystemClock()

    builder.Services
        .AddSingleton(configModule)
        .AddSingleton<Database>(fun _ -> EvilPlanner.Backend.Application.initDatabase lifetime configModule.EvilPlannerConfig) // TODO: Get rid of Lifetime and rely on container dispose management
        // .AddSingleton<Database>(fun _ -> database)
        .AddSingleton<IClock>(clock)
    |> ignore

    builder.Services.AddMvc() |> ignore

    builder

let private build lifetime (builder: WebApplicationBuilder) =
    let app = builder.Build()
    app.UseStaticFiles() |> ignore
    let router = createRouter lifetime app.Services
    app.UseRouting() |> ignore
    app.MapControllers() |> ignore
    useFreya router app
    app

let private run(app: WebApplication) = app.Run()

[<EntryPoint>]
let main(args: string[]): int =
    use appLifetime = new LifetimeDefinition()
    let lt = appLifetime.Lifetime
    let cfg =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()

    WebApplication.CreateBuilder(args)
    |> (configure lt cfg)
    |> (build lt)
    |> run

    0
