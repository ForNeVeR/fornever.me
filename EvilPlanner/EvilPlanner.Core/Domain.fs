namespace EvilPlanner.Core

open System

[<CLIMutable>]
type DailyQuote =
    { id : int64
      date : DateTime
      quotationId : int64 }

[<CLIMutable>]
type Quotation =
    { id : int64
      text : string
      source : string
      sourceUrl : string }
