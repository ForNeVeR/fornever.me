namespace ForneverMind.Models

open System

(*type PostModel () =
    member val CommentThreadId = "" with get, set
    member val Title = "" with get, set
    member val DateTime = DateTime.MinValue with get, set
    member val HtmlContent = "" with get, set
*)

type PostModel =
    {
        CommentThreadId : string
        Title : string
        DateTime: DateTime
        HtmlContent: string
    }
