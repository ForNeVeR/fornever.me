module ForneverMind.Program

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging

open ForneverMind.KestrelInterop

let private linkApplication env =
    let configuration = ConfigurationModule(env)
    let posts = PostsModule(configuration)
    let templates = TemplatingModule(configuration)
    let pages = PagesModule(posts, templates)
    RoutesModule(pages)

let private createRouter (builder : IApplicationBuilder) =
    let env = downcast builder.ApplicationServices.GetService typeof<IHostingEnvironment>
    let routesModule = linkApplication env
    routesModule.Router

let private useStaticFiles (app : IApplicationBuilder) =
    app.UseStaticFiles("/app")
        .UseStaticFiles("/images")
        .UseStaticFiles("/talks")

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
