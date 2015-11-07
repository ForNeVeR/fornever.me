namespace ForneverMind.Tests

open System
open System.IO

open Xunit

open ForneverMind
open ForneverMind.Models

type public MarkdownTests() =

    let compareResult input expected =
        use reader = new StringReader (input)
        let actual = Markdown.processReader reader

        Assert.Equal (expected, actual)

    [<Fact>]
    member __.EmptyDocument () =
        compareResult ""
            {
                Title = ""
                CommentThreadId = ""
                DateTime = DateTime.MinValue
                HtmlContent = Environment.NewLine
            }
