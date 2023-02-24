module EvilPlanner.Core.Storage

open System
open System.Threading

open LiteDB

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
        member _.Dispose() =
            connection.Dispose()
            lock.Dispose()

let openDatabase (config : Configuration) : Database =
    // TODO: Figure out how to escape the quotes in the path.
    let connectionString = $"Filename=%s{config.databasePath}; Exclusive=true; mode=Exclusive; upgrade=true"
    let connection = new LiteDatabase(connectionString)
    new Database(connection)
