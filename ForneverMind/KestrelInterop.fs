namespace ForneverMind.KestrelInterop

open System
open System.IO

open Freya.Core
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

type ApplicationConfiguration =
    { application : IApplicationBuilder -> unit
      logging : ILoggingBuilder -> unit
      services : IServiceCollection -> unit }

module ApplicationBuilder =
    let inline useFreya f (app:IApplicationBuilder)=
        let owin : OwinMidFunc = OwinMidFunc.ofFreya f
        app.UseOwin(fun p -> p.Invoke owin)

module WebHost =
    let private root = Directory.GetCurrentDirectory()
    let private webRoot = Path.Combine(root, "wwwroot")
    let create () = WebHostBuilder().UseContentRoot(root).UseWebRoot(webRoot).UseKestrel()
    let configure ({ application = application
                     logging = logging
                     services = services }) (b:IWebHostBuilder) =
        b.ConfigureServices(Action<_> services)
            .ConfigureLogging(Action<_> logging)
            .Configure(Action<_> application)
    let build (b:IWebHostBuilder) = b.Build()
    let run (wh:IWebHost) = wh.Run()
    let buildAndRun : IWebHostBuilder -> unit = build >> run
