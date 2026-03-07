// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Models

open ForneverMind.Core

type PostModel =
    {
        Meta : PostMetadata
        HtmlContent : string
    }
