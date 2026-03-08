// SPDX-FileCopyrightText: 2015-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module ForneverMind.Core.FileSystem

open TruePath

let IsPathInsideDirectory (directory: AbsolutePath, path: AbsolutePath): bool =
    directory.IsPrefixOf path
