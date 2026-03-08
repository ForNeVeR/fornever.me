// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Core

type IPostsProvider =
    abstract AllPosts: language: string -> PostMetadata[]
