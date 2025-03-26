// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace EvilPlanner.Core

open System

[<CLIMutable>]
type DailyQuote =
    { id : int64
      date : DateTime // TODO[#165]: Convert to DateOnly
      quotationId : int64 }

[<CLIMutable>]
type Quotation =
    { id : int64
      text : string
      source : string
      sourceUrl : string }
