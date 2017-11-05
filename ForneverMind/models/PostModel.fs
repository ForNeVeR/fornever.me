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
