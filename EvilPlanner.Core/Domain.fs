namespace EvilPlanner.Core

open System

type DailyQuote =
    { id : int64
      date : DateTime
      quotationId : int64 }

type Quotation =
    { id : int64
      text : string
      source : string
      sourceUrl : string }
