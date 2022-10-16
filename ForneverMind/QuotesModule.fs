namespace ForneverMind

open EvilPlanner.Backend
open EvilPlanner.Core.Storage
open Freya.Machines
open Freya.Machines.Http

type QuotesModule(database: Database) =
    member val QuoteByDate: HttpMachine =
        Quotes.quoteByDate database
