module EvilPlanner.Logic.QuoteLogic

open System
open System.Data
open System.Data.SqlClient

open LiteDB

open EvilPlanner.Core

let private dailyQuotes (db : LiteDatabase) = db.GetCollection<DailyQuote> "dailyQuotes"
let private quotations (db : LiteDatabase) = db.GetCollection<Quotation> "quotations"

let private toOption(x : 'T) : 'T option =
    if isNull (box x)
    then None
    else Some x

let private getDailyQuote (date : DateTime) db =
    (dailyQuotes db).FindOne(Query.EQ("date", BsonValue date))
    |> toOption
    |> Option.map (fun dq ->
        (quotations db).FindById (BsonValue dq.quotationId))

/// Creates quote for the current date. Can throw an UpdateException in case when the quote was
/// already created by the concurrent query.
let private createQuote db date =
    // TODO: Optimize this randomization
    let allQuotations = ResizeArray((quotations db).FindAll())
    let index = Random().Next(allQuotations.Count - 1)
    let quotation = allQuotations.[index]
    let dailyQuote = { id = 0L; date = date; quotationId = quotation.id }
    ignore <| (dailyQuotes db).Insert dailyQuote
    quotation

let getQuote (database : Storage.Database) (date : DateTime) : Quotation option =
    let today = DateTime.UtcNow.Date
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
