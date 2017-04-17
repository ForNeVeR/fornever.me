module ForneverMind.Program

open System.IO

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

open ForneverMind.KestrelInterop

let private fuseApplication cfg env =
    let configuration = ConfigurationModule(env, cfg)
    let posts = PostsModule(configuration)
    let rss = RssModule(configuration, posts)
    let templates = TemplatingModule(configuration)
    let pages = PagesModule(posts, templates)
    RoutesModule(pages, rss)

let private createRouter (builder : IApplicationBuilder) =
    let env = downcast builder.ApplicationServices.GetService typeof<IHostingEnvironment>
    let cfg =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build()
    let routesModule = fuseApplication cfg env
    routesModule.Router

let private useStaticFiles (app : IApplicationBuilder) =
    app.UseStaticFiles()

let private configureApplication app =
    let router = createRouter app
    useStaticFiles app
    |> ApplicationBuilder.useFreya router

let private configureLogger (logger : ILoggerFactory) =
    logger.AddConsole()

let configuration =
    { application = configureApplication >> ignore
      logging = configureLogger >> ignore }

[<EntryPoint>]
let main argv =
    WebHost.create ()
    |> WebHost.bindTo [|"http://localhost:5000"|]
    |> WebHost.configure configuration
    |> WebHost.buildAndRun

    0
