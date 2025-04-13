// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System
open System.Globalization
open System.IO
open System.Text

open ForneverMind.Markdown
open Markdig
open Markdig.Syntax

open ForneverMind.Models

type MarkdownModule(highlight : CodeHighlightModule) =
    let getMetadata block =
        match block with
        | None -> Map.empty
        | Some (b) ->
            let meta = MarkdownUtils.extractCode b
            meta.Split '\n'
            |> Seq.map (fun s -> s.Trim ())
            |> Seq.filter (not << String.IsNullOrEmpty)
            |> Seq.map (fun s ->
                match s.Split ([| ':' |], 2) with
                | [| key; value |] -> key.Trim (), value.Trim ()
                | _ -> failwithf "Cannot parse metadata line %A" s)
            |> Map.ofSeq

    let readMetadata language (fileName: string) markdownBlocks =
        let splitBy cond input =
            let before = ResizeArray()
            let after = ResizeArray()
            let mutable fillingBefore = true
            for x in input do
                if fillingBefore && (not << cond) x then
                    before.Add x
                else if fillingBefore && cond x then
                    fillingBefore <- false
                else
                    after.Add x
            before, after

        let isHorizontalRuler(x: Block) = x :? ThematicBreakBlock
        let isIndentedCode(x: Block) = x :? CodeBlock

        let metadataNodes, rest = splitBy isHorizontalRuler markdownBlocks
        let metadataBlock =
            metadataNodes
            |> Seq.filter isIndentedCode
            |> Seq.cast
            |> Seq.tryHead

        let metadata = getMetadata metadataBlock
        let getMeta key def =
            match Map.tryFind key metadata with
            | Some v -> v
            | None -> def

        let dateString = fileName.Substring (0, "yyyy-MM-dd".Length)
        let date = DateTime.ParseExact (dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture)

        {
            Url = sprintf "/%s/posts/%s.html" language fileName
            Date = date
            Title = getMeta "title" ""
            Description = getMeta "description" ""
            CommentUrl = getMeta "commentUrl" ""
        }, rest

    let getLanguageAndName (path : string) =
        let language = Path.GetFileName (Path.GetDirectoryName path)
        let name = Path.GetFileNameWithoutExtension path
        language, name

    let markdownPipeline =
        MarkdownPipelineBuilder()
            .UsePipeTables()
            .Build()

    member __.ProcessMetadata(filePath : string, reader : TextReader): PostMetadata =
        let document = Markdown.Parse(reader.ReadToEnd(), markdownPipeline)
        let (language, name) = getLanguageAndName filePath
        let metadata, _ = readMetadata language name document
        metadata

    member _.ProcessReader(filePath : string, reader : TextReader) =
        use target = new StringWriter ()
        let document = Markdown.Parse(reader.ReadToEnd(), markdownPipeline)
        let (language, name) = getLanguageAndName filePath
        let metadata, remainingBlocks = readMetadata language name document
        document.Clear() // to detach the blocks from it

        let documentWithoutMetadata = MarkdownDocument()
        remainingBlocks |> Seq.iter documentWithoutMetadata.Add

        use server = highlight.StartServer()
        let renderer = MarkdownHtmlRenderer(server, target, highlight)
        renderer.Render documentWithoutMetadata |> ignore

        {
            Meta = metadata
            HtmlContent = target.ToString()
        }

    member this.Render(filePath : string): Async<PostModel> =
        async {
            do! Async.SwitchToThreadPool ()

            use stream = new FileStream(filePath, FileMode.Open)
            use reader = new StreamReader(stream, Encoding.UTF8)
            return this.ProcessReader(filePath, reader)
        }
