// SPDX-FileCopyrightText: 2022-2023 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Controllers

open Microsoft.AspNetCore.Mvc

open ForneverMind

[<Route("/")>]
type PagesController() =
    inherit Controller()

    [<Route("/")>]
    member this.Get(): IActionResult =
        RedirectResult $"/{Common.defaultLanguage}/"
