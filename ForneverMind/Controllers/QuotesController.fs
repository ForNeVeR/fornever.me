namespace ForneverMind.Controllers

open Microsoft.AspNetCore.Mvc

[<Route("/plans/quote/")>]
type QuotesController() =
    inherit Controller()

    member this.Get(date: string) =
        this.Json date
