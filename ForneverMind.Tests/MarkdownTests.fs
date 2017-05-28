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
    code"
        {
            Meta =
                {
                    Url = "/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                }
            HtmlContent = "<pre><code class=\"hljs\"><span class=\"hljs-keyword\">test
</span>code
</code></pre>
"
        }

[<Fact>]
let ``Code language should be included as class`` () =
    compareResult "0001-01-01"
        "
---
```fsharp
let x = x
```"
        {
            Meta =
                {
                    Url = "/posts/0001-01-01.html"
                    Title = ""
                    Description = ""
                    Date = DateTime.MinValue
                }
            HtmlContent = "<pre><code class=\"hljs\"><span class=\"hljs-keyword\">let</span> x = x
</code></pre>
"
        }
