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

type Ext =
    static member singleOrDefaultAsync query =
        async {
            let! result =
                query
                |> QueryableExtensions.SingleOrDefaultAsync
                |> Async.AwaitTask
            return Option.ofObj result
        }

    static member singleOrDefaultAsync (query : DbRawSqlQuery<'a>) =
        async {
            let! result =
                query.SingleOrDefaultAsync()
                |> Async.AwaitTask
            return Option.ofObj result
        }

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
