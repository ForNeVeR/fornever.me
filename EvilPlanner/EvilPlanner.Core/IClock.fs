// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace EvilPlanner.Core

open System

type IClock =
    abstract member Today: unit -> DateOnly

type SystemClock() =
    interface IClock with
        member _.Today() = DateOnly.FromDateTime DateTime.UtcNow.Date
