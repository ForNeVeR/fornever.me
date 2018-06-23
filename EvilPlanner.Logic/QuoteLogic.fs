module EvilPlanner.Logic.QuoteLogic

open System
open System.Data
open System.Data.SqlClient

open FSharpx
open LiteDB

open EvilPlanner.Core

let private dailyQuotes (db : LiteDatabase) = db.GetCollection<DailyQuote> "dailyQuotes"
let private quotations (db : LiteDatabase) = db.GetCollection<Quotation> "quotations"

let private toOption (x : 'T) : 'T option =
    box x
    |> Option.ofObj
    |> Option.cast

let private getDailyQuote db (date : DateTime) =
    (dailyQuotes db).FindOne(Query.EQ("Date", BsonValue date))
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

let getQuote (config : Configuration) (date : DateTime) : Quotation option =
    let today = DateTime.UtcNow.Date
    use db = Storage.openDatabase config
    let existingQuote = getDailyQuote db date

    if today <> date
    then existingQuote
    else match existingQuote with
         | None -> Some (createQuote db today)
         | _ -> existingQuote
