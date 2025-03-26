// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind.Models

open System

type PostMetadata =
    {
        Url : string
        Title : string
        Description : string
        Date : DateTime
        CommentUrl : string
    }

type PostModel =
    {
        Meta : PostMetadata
        HtmlContent : string
    }
