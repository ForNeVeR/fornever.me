// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Core

open System

type PostMetadata =
    {
        Url : string
        Title : string
        Description : string
        Date : DateTime
        CommentUrl : string
    }
