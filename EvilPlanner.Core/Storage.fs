module EvilPlanner.Core.Storage

open LiteDB

let openDatabase (config : Configuration) =
    new LiteDatabase(config.databasePath)
