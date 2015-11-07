namespace ForneverMind.Models

open System

type PostModel =
    {
        CommentThreadId : string
        Title : string
        Date: DateTime
        HtmlContent: string
    }
