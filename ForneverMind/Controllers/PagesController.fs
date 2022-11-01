namespace ForneverMind.Controllers


open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open EvilPlanner.Core
open EvilPlanner.Core.Storage

[<Route("/")>]
type PagesController(logger: ILogger<QuotesController>, clock: IClock, database: Database) =
    inherit Controller()

    // [<Route("")>]
    member this.Get(): IActionResult =
        (*let success, date = DateOnly.TryParseExact(
            dateString,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None)
        if success then
            match QuoteLogic.getQuote clock database date with
            | None ->*) this.Json "xxx" (*
            | Some quote -> this.Json quote
        else
            logger.LogWarning($"Invalid date in request: {dateString}")
            this.BadRequest() *)
