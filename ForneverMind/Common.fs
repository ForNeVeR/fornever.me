// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Common

open System.IO

let defaultLanguage = "en"
let supportedLanguages = [| "en"; "ru" |]

let pathIsInsideDirectory directory path =
    let fullDirectory = Path.GetFullPath directory
    let fullPath = Path.GetFullPath path
    fullPath.StartsWith fullDirectory
