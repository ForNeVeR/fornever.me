module ForneverMind.Program

open System.IO

open JetBrains.Lifetimes
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open ForneverMind.KestrelInterop

let private fuseApplication lifetime (app: IApplicationBuilder) cfg env =
    let configuration = ConfigurationModule(env, cfg)
    let database = EvilPlanner.Backend.Application.initDatabase configuration.EvilPlannerConfig app
    let logger = app.ApplicationServices.GetRequiredService<ILogger<CodeHighlightModule>>()
    let highlight = CodeHighlightModule(lifetime, logger)
    let markdown = MarkdownModule(highlight)
    let posts = PostsModule(configuration, markdown)
    let rss = RssModule(configuration, posts)
    let templates = TemplatingModule(configuration)
    let pages = PagesModule(posts, templates, markdown)
    let quotes = QuotesModule database
    RoutesModule(pages, rss, quotes)

let private createRouter lifetime (builder : IApplicationBuilder) =
    let env = downcast builder.ApplicationServices.GetService typeof<IWebHostEnvironment>
    let cfg =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()

    let routesModule = fuseApplication lifetime builder cfg env
    routesModule.Router

let private useStaticFiles (app : IApplicationBuilder) =
    app.UseStaticFiles()

let private configureApplication lt app =
    let router = createRouter lt app
    useStaticFiles app
    |> ApplicationBuilder.useFreya router
    |> ignore

let private configureLogger(logger: ILoggingBuilder) =
    logger.AddConsole()

let private configuration =
    { Application = configureApplication
      Logging = configureLogger >> ignore }

[<EntryPoint>]
let main(args: string[]): int =
    use ld = new LifetimeDefinition()
    WebHost.create args
    |> WebHost.configure ld.Lifetime configuration
    |> WebHost.run ld

    0
