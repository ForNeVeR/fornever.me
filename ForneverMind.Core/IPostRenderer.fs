// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Core

open System.Threading.Tasks

type IPostRenderer =
    abstract Render: filePath: string -> Task<PostModel>
    abstract PostsPath: string
