namespace ForneverMind.Models

open System

type PostMetadata =
    {
        Url : string
        Title : string
        Description : string
        Date : DateTime
    }

type PostModel =
    {
        Meta : PostMetadata
        HtmlContent : string
    }
