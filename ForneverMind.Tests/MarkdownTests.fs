namespace ForneverMind.Tests

open System
open System.IO

open Xunit

open ForneverMind
open ForneverMind.Models

type public MarkdownTests() =

    let compareResult fileName input expected =
        use reader = new StringReader (input)
        let actual = Markdown.processReader fileName reader

        Assert.Equal (expected, actual)

    [<Fact>]
    member __.EmptyDocument () =
        compareResult "0001-01-01"
            "
---
"
            {
                Meta =
                    {
                        Url = "/posts/0001-01-01.html"
                        Title = ""
                        Description = ""
                        CommentThreadId = "/posts/0001-01-01.html"
                        Date = DateTime.MinValue
                    }
                HtmlContent = ""
            }

    [<Fact>]
    member __.SimpleMetadata () =
        compareResult "2015-01-01"
            "
    title: Фильтры исключений в CLR
    id: /posts/2013-09-01-clr-exception-filters_ru.html
    description: Описание механизма фильтров исключений, доступного для некоторых языков CLR.
---
content
"
            {
                Meta =
                    {
                        Url = "/posts/2015-01-01.html"
                        Date = DateTime(2015, 1, 1)
                        Title = "Фильтры исключений в CLR"
                        Description = "Описание механизма фильтров исключений, доступного для некоторых языков CLR."
                        CommentThreadId = "/posts/2013-09-01-clr-exception-filters_ru.html"
                    }
                HtmlContent = "<p>content</p>" + Environment.NewLine
            }

    [<Fact>]
    member __.LegacyCommentId () =
        compareResult "0001-01-01_File_Name"
            "
---
"
            {
                Meta =
                    {
                        Url = "/posts/0001-01-01_File_Name.html"
                        Date = DateTime.MinValue
                        Title = ""
                        Description = ""
                        CommentThreadId = "/posts/0001-01-01_File_Name.html"
                    }
                HtmlContent = ""
            }
