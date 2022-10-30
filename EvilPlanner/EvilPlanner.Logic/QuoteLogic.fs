module EvilPlanner.Logic.QuoteLogic

open System

open LiteDB

open EvilPlanner.Core

let private dailyQuotes (db : LiteDatabase) = db.GetCollection<DailyQuote> "dailyQuotes"
let private quotations (db : LiteDatabase) = db.GetCollection<Quotation> "quotations"

let private toOption(x : 'T) : 'T option =
    if isNull (box x)
    then None
    else Some x

let private toDateTime(date: DateOnly) =
    date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero), DateTimeKind.Utc)

let internal getDailyQuote (date: DateOnly) (db: LiteDatabase): Quotation option =
    let date = toDateTime date
    (dailyQuotes db).FindOne(Query.EQ("date", BsonValue date))
    |> toOption
    |> Option.map (fun dq ->
        (quotations db).FindById (BsonValue dq.quotationId))

/// Creates quote for the current date.
let private createQuote db date =
    let date = toDateTime date
    // TODO[#141]: Optimize this randomization
    let allQuotations = ResizeArray((quotations db).FindAll())
    let index = Random().Next(allQuotations.Count - 1)
    let quotation = allQuotations.[index]
    let dailyQuote = { id = 0L; date = date; quotationId = quotation.id }
    ignore <| (dailyQuotes db).Insert dailyQuote
    quotation

let getQuote (clock: IClock) (database: Storage.Database) (date: DateOnly): Quotation option =
    let today = clock.Today()
    let existingQuote = database.ReadOnlyTransaction (getDailyQuote date)
    if today <> date || Option.isSome existingQuote
    then existingQuote
    else
        database.ReadWriteTransaction(fun db ->
            // Retry the select in case another transaction has already created the quote:
            let existingQuote = getDailyQuote date db
            match existingQuote with
            | None -> Some (createQuote db today)
            | _ -> existingQuote
        )
