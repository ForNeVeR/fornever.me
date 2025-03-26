// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace EvilPlanner.Tests

open System
open System.IO
open System.Reflection
open System.Threading.Tasks

open EvilPlanner.Core.Storage
open Xunit

open EvilPlanner.Core
open ForneverMind.TestFramework.StorageUtils

type ConcurrencyTest() =
    let config =
        let directory = AssemblyUtils.assemblyDirectory(Assembly.GetExecutingAssembly())
        { databasePath = Path.Combine(directory, "testDb.dat") }

    let clock: IClock = {
        new IClock with
            member _.Today() = DateOnly(2023, 2, 26)
    }

    [<Fact>]
    member __.ConcurrencyTest() : Task<unit> =
        do
            reinitializeDatabase config
            use database = new Database(config)
            database.ReadWriteTransaction clearDailyQuotes

        let concurrencyLevel = 20
        async {
            use database = new Database(config)
            let tasks =
                seq { 1 .. concurrencyLevel }
                |> Seq.map (fun _ -> async { return executeTransaction clock database })
            let! quotes = Async.Parallel tasks

            let count = database.ReadOnlyTransaction countDailyQuotes
            Assert.Equal (1, count)

            let successCount =
                quotes
                |> Seq.filter Option.isSome
                |> Seq.length
            Assert.Equal (concurrencyLevel, successCount)
        } |> Async.StartAsTask
