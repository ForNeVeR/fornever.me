module ForneverMind.TestFramework.StorageUtils

open System.IO

open LiteDB

open EvilPlanner.Core
open EvilPlanner.Core.Storage
open EvilPlanner.Logic.QuoteLogic

let private dailyQuotes(db: LiteDatabase) = db.GetCollection<DailyQuote>("dailyQuotes")
let private getDailyQuotes(db: LiteDatabase) = (dailyQuotes db).FindAll()

let reinitializeDatabase(config: Configuration): unit =
    if File.Exists config.databasePath
    then File.Delete config.databasePath

    Migrations.migrateDatabase config

let clearDailyQuotes(db: LiteDatabase): unit =
    ignore <| (dailyQuotes db).DeleteAll()

let executeTransaction (clock: IClock) (database: Database): Quotation option =
    let today = clock.Today()
    getQuote clock database today

let countDailyQuotes(db: LiteDatabase): int =
    Seq.length(getDailyQuotes db)
