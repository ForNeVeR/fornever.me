module EvilPlanner.Core.Migrations

open System
open System.IO
open System.Reflection

open CsvHelper
open LiteDB

open EvilPlanner.Core.Storage

let private prepareHeader (s : string) = sprintf "%c%s" (Char.ToUpperInvariant s.[0]) (s.Substring 1)

let private readCsv resourceFileName =
    let assembly = Assembly.GetExecutingAssembly()
    let resourceName = sprintf "%s.%s" (assembly.GetName().Name) resourceFileName
    use resource = assembly.GetManifestResourceStream resourceName
    use reader = new StreamReader(resource)
    let config = Configuration.Configuration(Delimiter = ";", PrepareHeaderForMatch = Func<_, _> prepareHeader)
    use csv = new CsvReader(reader, config)
    ResizeArray(csv.GetRecords())

let private seedDatabase(db : LiteDatabase) =
    let quotationData = readCsv "Migrations.Quotations.csv"
    let dailyQuoteData = readCsv "Migrations.DailyQuotes.csv"
    let quotations = db.GetCollection<Quotation> "quotations"
    let dailyQuotes = db.GetCollection<DailyQuote> "dailyQuotes"
    ignore <| dailyQuotes.EnsureIndex "date"
    ignore <| quotations.Insert quotationData
    ignore <| dailyQuotes.Insert dailyQuoteData

let migrateDatabase(configuration : Configuration) : unit =
    ignore <| Directory.CreateDirectory(Path.GetDirectoryName configuration.databasePath)
    use database = new Database(configuration)
    database.ReadWriteTransaction(fun db ->
        if db.UserVersion = 0
        then
            seedDatabase db
            db.UserVersion <- 1
    )
