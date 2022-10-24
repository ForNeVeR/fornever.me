namespace ForneverMind.KestrelInterop

open Freya.Core
open JetBrains.Lifetimes
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging

type ApplicationConfiguration = {
      Application: Lifetime -> IApplicationBuilder -> unit
      Logging: ILoggingBuilder -> unit
}

module ApplicationBuilder =
    let inline useFreya f (app:IApplicationBuilder)=
        let owin : OwinMidFunc = OwinMidFunc.ofFreya f
        app.UseOwin(fun p -> p.Invoke owin)

module WebHost =
    let create(args: string[]) =
        WebApplication.CreateBuilder(args)
    let configure (lifetime: Lifetime)
                  ({ Application = application
                     Logging = logging }: ApplicationConfiguration)
                  (b: WebApplicationBuilder): WebApplication =
        logging b.Logging
        b.WebHost.ConfigureKestrel(fun opts -> opts.AllowSynchronousIO <- true) |> ignore
        let app = b.Build()
        application lifetime app
        app
    let run (ld: LifetimeDefinition) (app: WebApplication): unit =
        try
            app.Run()
        finally
            ld.Terminate()
