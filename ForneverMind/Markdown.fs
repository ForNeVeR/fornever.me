module ForneverMind.Markdown

open System
open System.Globalization
open System.IO
open System.Text

open CommonMark
open CommonMark.Formatters
open CommonMark.Syntax

open ForneverMind.Models

type Formatter (target, settings) =
    inherit HtmlFormatter (target, settings)
    let skippedCode = ref false
    let skippedRuler = ref false

    override __.WriteBlock (block, isOpening, isClosing, ignoreChildNodes) =
        let skipCode = not (!skippedCode) && block.Tag = BlockTag.IndentedCode
        let skipRuler = not (!skippedRuler) && block.Tag = BlockTag.HorizontalRuler
        match skipCode, skipRuler with
        | (true, _) -> skippedCode := true; ()
        | (_, true) -> skippedRuler := true; ()
        | _ ->
            ignoreChildNodes <- false
            base.WriteBlock (block, isOpening, isClosing, ref ignoreChildNodes)

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

let private legacyCommentId fileName =
    sprintf "/posts/%s.html" fileName

let private readMetadata (fileName : string) documentNodes =
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
    let getMeta key def =
        match Map.tryFind key metadata with
        | Some v -> v
        | None -> def

    let dateString = fileName.Substring (0, "yyyy-MM-dd".Length)
    let date = DateTime.ParseExact (dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture)

    {
        Title = getMeta "title" ""
        CommentThreadId = getMeta "id" <| legacyCommentId fileName
        Date = date
    }

let getParseSettings =
    let settings = CommonMarkSettings.Default.Clone ()
    settings.OutputDelegate <- fun doc output settings -> Formatter(output, settings).WriteDocument(doc)
    settings

let processReader (fileName : string) (reader : TextReader)  =
    use target = new StringWriter ()
    let document = CommonMarkConverter.Parse reader
    let metadata = readMetadata (Path.GetFileNameWithoutExtension fileName) <| document.AsEnumerable ()
    let settings = getParseSettings

    CommonMarkConverter.ProcessStage3 (document, target, settings)

    {
        Meta = metadata
        HtmlContent = target.ToString ()
    }


let render (fileName : string): Async<PostModel> =
    async {
        do! Async.SwitchToThreadPool ()

        use reader = new StreamReader (fileName, Encoding.UTF8)
        return processReader fileName reader
    }
