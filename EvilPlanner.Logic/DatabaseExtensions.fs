module EvilPlanner.Logic.DatabaseExtensions

open System.Data.Entity
open System.Data.Entity.Infrastructure
open System.Linq

open EvilPlanner.Data
open EvilPlanner.Data.Entities

let headAsync query =
    query
    |> QueryableExtensions.FirstAsync
    |> Async.AwaitTask

type SingleOrDefaultAsync = SingleOrDefaultAsync with
    static member ($) (SingleOrDefaultAsync, query) =
        async {
            let! result =
                query
                |> QueryableExtensions.SingleOrDefaultAsync
                |> Async.AwaitTask
            return Option.ofObj result
        }

    static member ($) (SingleOrDefaultAsync, query : DbRawSqlQuery<'a>) =
        async {
            let! result =
                query.SingleOrDefaultAsync()
                |> Async.AwaitTask
            return Option.ofObj result
        }

let inline singleOrDefaultAsync x = SingleOrDefaultAsync $ x

let countAsync query =
    query
    |> QueryableExtensions.CountAsync
    |> Async.AwaitTask

let toListAsync query =
    query
    |> QueryableExtensions.ToListAsync
    |> Async.AwaitTask

let saveChangesAsync (context : DbContext) =
    async {
        let! updated = context.SaveChangesAsync() |> Async.AwaitTask
        ignore updated
    }
