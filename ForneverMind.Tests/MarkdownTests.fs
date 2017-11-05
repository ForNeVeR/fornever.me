module ForneverMind.Tests.MarkdownTests

open System
open System.IO

open Microsoft.AspNetCore.NodeServices
open Microsoft.Extensions.DependencyInjection
open Xunit

open ForneverMind
open ForneverMind.Models

let markdown =
    let services = ServiceCollection()
    services.AddNodeServices(fun o -> o.ProjectPath <- Directory.GetCurrentDirectory())
    let serviceProvider = services.BuildServiceProvider()
    let node = serviceProvider.GetRequiredService<INodeServices>()
    let highlight = CodeHighlightModule(node)
    MarkdownModule(highlight)

let private normalizeLineEndings (s : string) = s.Replace(Environment.NewLine, "\n")
let private normalizeHtmlContent ({ HtmlContent = content } as item) =
    { item with HtmlContent = normalizeLineEndings content }

let compareResult fileName (input : string) expected =
    use reader = new StringReader (normalizeLineEndings input)
    let actual = markdown.ProcessReader(fileName, reader)

    Assert.Equal (normalizeHtmlContent expected, normalizeHtmlContent actual)

[<Fact>]
let ``Empty document should be parsed`` () =
    compareResult "ru/0001-01-01"
        "
---
"
        {
            Meta =
                {
                    Url = "/ru/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                    CommentUrl = ""
                }
            HtmlContent = ""
        }

[<Fact>]
let ``Simple metadata should be parsed`` () =
    compareResult "ru/2015-01-01"
            "
    title: Фильтры исключений в CLR
    description: Описание механизма фильтров исключений, доступного для некоторых языков CLR.
---
content
"
        {
            Meta =
                {
                    Url = "/ru/posts/2015-01-01.html"
                    Date = DateTime(2015, 1, 1)
                    Title = "Фильтры исключений в CLR"
                    Description = "Описание механизма фильтров исключений, доступного для некоторых языков CLR."
                    CommentUrl = ""
                }
            HtmlContent = "<p>content</p>" + Environment.NewLine
        }

[<Fact>]
let ``Legacy comment id should be parsed`` () =
    compareResult "ru/0001-01-01_File_Name"
        "
---
"
        {
            Meta =
                {
                    Url = "/ru/posts/0001-01-01_File_Name.html"
                    Date = DateTime.MinValue
                    Title = ""
                    Description = ""
                    CommentUrl = ""
                }
            HtmlContent = ""
        }

[<Fact>]
let ``Code block should be rendered with microlight class`` () =
    compareResult "ru/0001-01-01"
        "
---
    test
    code"
        {
            Meta =
                {
                    Url = "/ru/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                    CommentUrl = ""
                }
            HtmlContent = "<pre><code class=\"hljs\"><span class=\"hljs-keyword\">test
</span>code
</code></pre>
"
        }

[<Fact>]
let ``Code language should be included as class`` () =
    compareResult "ru/0001-01-01"
        "
---
```fsharp
let x = x
```"
        {
            Meta =
                {
                    Url = "/ru/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                    CommentUrl = ""
                }
            HtmlContent = "<pre><code class=\"hljs\"><span class=\"hljs-keyword\">let</span> x = x
</code></pre>
"
        }

[<Fact>]
let ``Identifier is extracted from the metadata`` () =
    compareResult "/something/en/2017-01-01.html"
            "
    title: Фильтры исключений в CLR
    description: Описание механизма фильтров исключений, доступного для некоторых языков CLR.
    commentUrl: https://fornever.me/posts/2013-09-01-clr-exception-filters_ru.html
---
content
"
        {
            Meta =
                {
                    Url = "/en/posts/2017-01-01.html"
                    Date = DateTime(2017, 1, 1)
                    Title = "Фильтры исключений в CLR"
                    Description = "Описание механизма фильтров исключений, доступного для некоторых языков CLR."
                    CommentUrl = "https://fornever.me/posts/2013-09-01-clr-exception-filters_ru.html"
                }
            HtmlContent = "<p>content</p>" + Environment.NewLine
        }
