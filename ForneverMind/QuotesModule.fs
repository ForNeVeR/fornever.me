namespace ForneverMind

open Freya.Machines
open Freya.Machines.Http

open EvilPlanner.Backend
open EvilPlanner.Core
open EvilPlanner.Core.Storage

type QuotesModule(clock: IClock, database: Database) =
    member val QuoteByDate: HttpMachine =
        Quotes.quoteByDate clock database
