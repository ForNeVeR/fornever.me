module EvilPlanner.Logic.DatabaseExtensions

open System.Data.Entity

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

let saveChangesAsync (context : DbContext) =
    async {
        let! updated = context.SaveChangesAsync() |> Async.AwaitTask
        ignore updated
    }
