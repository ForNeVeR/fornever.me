module EvilPlanner.Storage

open System
open System.IO

open LiteDB

open EvilPlanner.Data.Entities

type DailyQuote =
    { id : int64
      date : DateTime
      quotationId : int64 }

let private seedDatabase(db : LiteDatabase) =
    let quotationData = [| Quotation(Id = 1L, Text = "Text", Source = "Source", SourceUrl = "SourceUrl") |]
    let dailyQuoteData = [| { id = 1L; date = DateTime.Today; quotationId = 1L } |]
    let quotations = db.GetCollection<Quotation> "quotations"
    let dailyQuotes = db.GetCollection<DailyQuote> "dailyQuotes"
    ignore <| quotations.Insert quotationData
    ignore <| dailyQuotes.Insert dailyQuoteData

let migrateDatabase(configuration : Configuration) : Async<unit> =
    async {
        ignore <| Directory.CreateDirectory(Path.GetDirectoryName configuration.databasePath)
        use db = new LiteDatabase(configuration.databasePath)
        if db.Engine.UserVersion = 0us
        then
            seedDatabase db
            db.Engine.UserVersion <- 1us
    }
