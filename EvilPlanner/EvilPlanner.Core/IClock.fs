namespace EvilPlanner.Core

open System

type IClock =
    abstract member Today: unit -> DateOnly

type SystemClock() =
    interface IClock with
        member _.Today() = DateOnly.FromDateTime DateTime.UtcNow.Date
