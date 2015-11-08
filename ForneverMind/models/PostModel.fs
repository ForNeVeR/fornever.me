namespace ForneverMind.Models

open System

type PostMetadata =
    {
        CommentThreadId : string
        Title : string
        Date : DateTime
    }

type PostModel =
    {
        Meta : PostMetadata
        HtmlContent : string
    }
