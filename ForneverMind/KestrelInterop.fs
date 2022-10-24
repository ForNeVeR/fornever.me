namespace ForneverMind.KestrelInterop

open System
open System.IO

open Freya.Core
open JetBrains.Lifetimes
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging

type WebHostConfiguration = {
    Configuration: string
    ContentRoot: string
    ApplicationName: string
}

type ApplicationConfiguration =
    { application : Lifetime -> IApplicationBuilder -> unit
      logging : ILoggingBuilder -> unit }

module ApplicationBuilder =
    let inline useFreya f (app:IApplicationBuilder)=
        let owin : OwinMidFunc = OwinMidFunc.ofFreya f
        app.UseOwin(fun p -> p.Invoke owin)

module WebHost =
    let create(args: WebHostConfiguration) =
        let webRoot = Path.Combine(args.ContentRoot, "wwwroot")
        WebHostBuilder().UseContentRoot(args.ContentRoot).UseWebRoot(webRoot).UseKestrel()
    let configure (lifetime: Lifetime)
                  ({ application = application
                     logging = logging }: ApplicationConfiguration)
                  (b: IWebHostBuilder): IWebHostBuilder =
        b.ConfigureLogging(Action<_> logging)
            .Configure(Action<_>(application lifetime))
            .ConfigureKestrel(fun opts -> opts.AllowSynchronousIO <- true)
    let build (b:IWebHostBuilder) = b.Build()
    let run (ld: LifetimeDefinition) (wh: IWebHost) =
        try
            wh.Run()
        finally
            ld.Terminate()
    let buildAndRun(ld: LifetimeDefinition): IWebHostBuilder -> unit = build >> run ld
