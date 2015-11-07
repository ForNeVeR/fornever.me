module ForneverMind.Markdown

open System
open System.Collections.Generic
open System.Globalization
open System.IO
open System.Text

open CommonMark
open CommonMark.Formatters
open CommonMark.Syntax

open ForneverMind.Models

type Formatter (target, settings, ignoredBlocks : HashSet<Block>) =
    inherit HtmlFormatter (target, settings) with
    override __.WriteBlock (block, isOpening, isClosing, ignoreChildNodes) =
        if not <| ignoredBlocks.Contains block
        then base.WriteBlock (block, isOpening, isClosing, ref ignoreChildNodes)
        else ()

let private getMetadata (block : EnumeratorEntry option) =
    match block with
    | None -> Map.empty
    | Some (b) ->
        let meta = b.Block.StringContent.ToString ()
        meta.Split('\n')
        |> Seq.map (fun s -> s.Trim())
        |> Seq.filter (not << String.IsNullOrEmpty)
        |> Seq.map (fun s ->
            match s.Split(':') with
            | [| key; value |] -> key.Trim(), value.Trim()
            | _ -> failwithf "Cannot parse metadata line %A" s)
        |> Map.ofSeq

let private readMetadata dateString documentNodes =
    let takeUntil cond seq =
        let found = ref false
        seq
        |> Seq.takeWhile (fun i ->
            let ret = not !found
            found := not <| cond i
            ret)

    let isBlockOfType tag (b : EnumeratorEntry) = (not << isNull) b.Block && b.Block.Tag = tag
    let isHorizontalRuler = isBlockOfType BlockTag.HorizontalRuler
    let isIndentedCode = isBlockOfType BlockTag.IndentedCode

    let metadataNodes = takeUntil (not << isHorizontalRuler) documentNodes
    let metadataBlock =
        metadataNodes
        |> Seq.filter isIndentedCode
        |> Seq.tryHead

    let metadata = getMetadata metadataBlock
    let getMeta key =
        match Map.tryFind key metadata with
        | Some v -> v
        | None -> ""

    let date = DateTime.ParseExact (dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture)

    {
        Title = getMeta "title"
        CommentThreadId = getMeta "id"
        Date = date
        HtmlContent = ""
    }, metadataNodes

let getParseSettings metadataNodes =
    let settings = CommonMarkSettings.Default.Clone ()
    settings.OutputDelegate <- fun doc output settings -> Formatter(output, settings, metadataNodes).WriteDocument(doc)
    settings

let processReader (fileName : string) (reader : TextReader)  =
    use target = new StringWriter ()
    let document = CommonMarkConverter.Parse reader
    let dateString = fileName.Substring (0, "yyyy-MM-dd".Length)
    let post, metadataNodes = readMetadata dateString <| document.AsEnumerable ()
    let metadataBlocks =
        metadataNodes
        |> Seq.map (fun n -> n.Block)
        |> HashSet
    let settings = getParseSettings metadataBlocks


    CommonMarkConverter.ProcessStage3 (document, target, settings)

    { post with HtmlContent = target.ToString () }


let render (fileName : string): Async<PostModel> =
    async {
        do! Async.SwitchToThreadPool ()

        use reader = new StreamReader (fileName, Encoding.UTF8)
        return processReader fileName reader
    }
