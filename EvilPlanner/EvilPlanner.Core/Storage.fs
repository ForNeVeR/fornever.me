module EvilPlanner.Core.Storage

open System
open System.Threading

open LiteDB

type Database(config: Configuration) =
    let lock = new ReaderWriterLockSlim()

    // TODO[#179]: Figure out how to escape the quotes in the path.
    let connectionString = $"Filename={config.databasePath}; Exclusive=true; mode=Exclusive; upgrade=true"
    let connection =
        let c = new LiteDatabase(connectionString)
        c.Pragma("UTC_DATE", true) |> ignore
        c

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
