namespace ForneverMind.Controllers

open System
open System.Globalization

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open EvilPlanner.Core
open EvilPlanner.Core.Storage
open EvilPlanner.Logic

[<Route("/plans/quote/")>]
type QuotesController(logger: ILogger<QuotesController>, clock: IClock, database: Database) =
    inherit Controller()

    [<Route("{dateString}")>]
    member this.Get(dateString: string): IActionResult =
        let success, date = DateOnly.TryParseExact(
            dateString,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None)
        if success then
            match QuoteLogic.getQuote clock database date with
            | None -> this.NotFound()
            | Some quote -> this.Json quote
        else
            logger.LogWarning($"Invalid date in request: {dateString}")
            this.BadRequest()
