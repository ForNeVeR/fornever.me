namespace ForneverMind.KestrelInterop

open System

open Freya.Core
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

type ApplicationConfiguration =
    { application : IApplicationBuilder -> unit
      logging : ILoggerFactory -> unit }

module ApplicationBuilder =
    let inline useFreya f (app:IApplicationBuilder)=
        let owin : OwinMidFunc = OwinMidFunc.ofFreya f
        app.UseOwin(fun p -> p.Invoke owin)

module WebHost =
    let create () = WebHostBuilder().UseKestrel()
    let bindTo urls (b:IWebHostBuilder) = b.UseUrls urls
    let configure ({ application = application; logging = logging }) (b:IWebHostBuilder) =
        b.ConfigureLogging(Action<_> logging)
            .Configure(Action<_> application)
    let build (b:IWebHostBuilder) = b.Build()
    let run (wh:IWebHost) = wh.Run()
    let buildAndRun : IWebHostBuilder -> unit = build >> run
