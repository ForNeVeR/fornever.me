// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Core

open System.Threading.Tasks
open TruePath

type IPostRenderer =
    abstract Render: filePath: AbsolutePath -> Task<PostModel>
    abstract PostsPath: AbsolutePath
