module EvilPlanner.Storage

open System
open System.IO
open System.Reflection

open CsvHelper
open LiteDB

open EvilPlanner.Data.Entities

type DailyQuote =
    { id : int64
      date : DateTime
      quotationId : int64 }

let private prepareHeader (s : string) = sprintf "%c%s" (Char.ToUpperInvariant s.[0]) (s.Substring 1)

let private readCsv resourceName =
    let assembly = Assembly.GetExecutingAssembly()
    use resource = assembly.GetManifestResourceStream resourceName
    use reader = new StreamReader(resource)
    let config = Configuration.Configuration(Delimiter = ";", PrepareHeaderForMatch = Func<_, _> prepareHeader)
    use csv = new CsvReader(reader, config)
    ResizeArray(csv.GetRecords())

let private seedDatabase(db : LiteDatabase) =
    let quotationData = readCsv "Quotations.csv"
    let dailyQuoteData = readCsv "DailyQuotes.csv"
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
