module EvilPlanner.Logic.DatabaseExtensions

open System.Data.Entity
open System.Linq

let headAsync query =
    query
    |> QueryableExtensions.FirstAsync
    |> Async.AwaitTask

let headOrDefaultAsync query =
    async {
        let! result =
            query
            |> QueryableExtensions.FirstOrDefaultAsync
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
