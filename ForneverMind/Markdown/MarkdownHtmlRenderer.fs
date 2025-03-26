// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System.IO

open ForneverMind.Markdown
open Markdig.Extensions.Tables
open Markdig.Renderers
open Markdig.Renderers.Html
open Markdig.Syntax

type HighlightJsCodeBlockRenderer(server: Server, highlight: CodeHighlightModule) =
    inherit HtmlObjectRenderer<CodeBlock>()

    let getCodeLanguage: CodeBlock -> string option = function
    | :? FencedCodeBlock as block -> Some block.Info
    | _ -> None

    override this.Write(renderer: HtmlRenderer, obj: CodeBlock): unit =
        let language = getCodeLanguage obj
        let sourceCode = MarkdownUtils.extractCode obj
        let renderedCode: string = Async.RunSynchronously <| highlight.Highlight(server, language, sourceCode)

        renderer
            .EnsureLine()
            .Write("<pre><code class=\"hljs\">")
            .Write(renderedCode)
            .WriteLine("</code></pre>")
        |> ignore

type MarkdownHtmlRenderer(server: Server, writer: TextWriter, highlight: CodeHighlightModule) as this =
    inherit HtmlRenderer(writer)

    do this.ObjectRenderers.RemoveAll(fun x -> x :? CodeBlockRenderer) |> ignore
    do this.ObjectRenderers.Add(HighlightJsCodeBlockRenderer(server, highlight))
    do this.ObjectRenderers.Add(HtmlTableRenderer())
