namespace ForneverMind.Controllers

open Microsoft.AspNetCore.Mvc

open ForneverMind

[<Route("/")>]
type PagesController() =
    inherit Controller()

    [<Route("/")>]
    member this.Get(): IActionResult =
        RedirectResult $"/{Common.defaultLanguage}/"
