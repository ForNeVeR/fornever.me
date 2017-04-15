namespace ForneverMind.KestrelInterop

open Freya.Core
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

module ApplicationBuilder =
    let inline useFreya f (app:IApplicationBuilder)=
        let owin : OwinMidFunc = OwinMidFunc.ofFreya f
        app.UseOwin(fun p -> p.Invoke owin)

    let inline useStaticFiles (app : IApplicationBuilder) =
        app.UseStaticFiles("/app")
        app.UseStaticFiles("/images")
        app.UseStaticFiles("/talks")

module WebHost =
    let create () = WebHostBuilder().UseKestrel()
    let bindTo urls (b:IWebHostBuilder) = b.UseUrls urls
    let configure (f : IApplicationBuilder -> IApplicationBuilder) (b:IWebHostBuilder) =
        b.Configure (System.Action<_> (f >> ignore))
    let build (b:IWebHostBuilder) = b.Build()
    let run (wh:IWebHost) = wh.Run()
    let buildAndRun : IWebHostBuilder -> unit = build >> run
