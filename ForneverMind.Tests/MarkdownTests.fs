module ForneverMind.Tests.MarkdownTests

open System
open System.IO

open Xunit

open ForneverMind
open ForneverMind.Models

let compareResult fileName input expected =
    use reader = new StringReader (input)
    let actual = Markdown.processReader fileName reader

    Assert.Equal (expected, actual)

[<Fact>]
let ``Empty document should be parsed`` () =
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
                    Date = DateTime.MinValue
                }
            HtmlContent = ""
        }

[<Fact>]
let ``Simple metadata should be parsed`` () =
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
                }
            HtmlContent = "<p>content</p>" + Environment.NewLine
        }

[<Fact>]
let ``Legacy comment id should be parsed`` () =
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
                }
            HtmlContent = ""
        }

[<Fact>]
let ``Code block should be rendered with microlight class`` () =
    compareResult "0001-01-01"
        "
---
    test
    code
"
        {
            Meta =
                {
                    Url = "/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                }
            HtmlContent = "<pre><code class=\"microlight\">test
code
</code></pre>
"
        }

[<Fact>]
let ``Inline code should be rendered with microlight class`` () =
    compareResult "0001-01-01"
        "
---
`test\`
"
        {
            Meta =
                {
                    Url = "/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                }
            HtmlContent = "<p><code class=\"microlight\">test\</code></p>
"
        }
