namespace ForneverMind.Models

open System

type PostMetadata =
    {
        Url : string
        CommentThreadId : string
        Title : string
        Description : string
        Date : DateTime
    }

type PostModel =
    {
        Meta : PostMetadata
        HtmlContent : string
    }
