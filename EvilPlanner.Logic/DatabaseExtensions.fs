module EvilPlanner.Logic.DatabaseExtensions

open System
open System.Data.Entity.Core
open System.Data.Entity.Infrastructure
open System.Data.Entity
open System.Data.SqlClient
open System.Linq

open FSharpx.Option

open EvilPlanner.Data
open EvilPlanner.Data.Entities

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

let rec (|SqlException|_|) (exn : Exception) =
    match exn with
    | :? AggregateException ->
        match exn.InnerException with
        | (SqlException se) -> Some se
        | _ -> None
    | :? DbUpdateException ->
        maybe {
            let! due = if exn :? DbUpdateException then Some exn else None
            let! ue = if due.InnerException :? UpdateException then Some due.InnerException else None
            let! se = if ue.InnerException :? SqlException then Some (ue.InnerException :?> SqlException) else None
            return se
        }
    | _ -> None

let headAsync query =
    query
    |> QueryableExtensions.FirstAsync
    |> Async.AwaitTask

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
