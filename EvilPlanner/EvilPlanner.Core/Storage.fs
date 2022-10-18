module EvilPlanner.Core.Storage

open System
open System.Threading

open LiteDB
open LiteDB.FSharp

type Database internal (connection : LiteDatabase) =
    let lock = new ReaderWriterLockSlim()

    member __.ReadOnlyTransaction<'T>(action : LiteDatabase -> 'T) : 'T =
        lock.EnterReadLock()
        try
            action connection
        finally
            lock.ExitReadLock()

    member __.ReadWriteTransaction<'T>(action : LiteDatabase -> 'T) : 'T =
        lock.EnterWriteLock()
        try
            action connection
        finally
            lock.ExitWriteLock()

    interface IDisposable with
        member __.Dispose() =
            connection.Dispose()
            lock.Dispose()

let private mapper = FSharpBsonMapper()
let openDatabase (config : Configuration) : Database =
    let connectionString = sprintf "Filename=%s; Exclusive=true; mode=Exclusive" config.databasePath
    let connection = new LiteDatabase(connectionString, mapper)
    new Database(connection)
