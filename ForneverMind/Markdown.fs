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

    override this.WriteBlock (block, isOpening, isClosing, ignoreChildNodes) =
        let skipCode = not !skippedCode && block.Tag = BlockTag.IndentedCode
        let skipRuler = not !skippedRuler && block.Tag = BlockTag.ThematicBreak
        match skipCode, skipRuler with
        | (true, _) -> skippedCode := true; ()
        | (_, true) -> skippedCode := true; skippedRuler := true; ()
        | _ ->
            match block.Tag with
            | BlockTag.FencedCode | BlockTag.IndentedCode ->
                ignoreChildNodes <- true
                this.EnsureNewLine ()
                this.Write "<pre><code class=\"microlight\">"
                this.WriteEncodedHtml block.StringContent
                this.WriteLine "</code></pre>"
            | _ ->
                ignoreChildNodes <- false
                base.WriteBlock (block, isOpening, isClosing, ref ignoreChildNodes)

let private getMetadata (block : EnumeratorEntry option) =
    match block with
    | None -> Map.empty
    | Some (b) ->
        let meta = b.Block.StringContent.ToString ()
        meta.Split '\n'
        |> Seq.map (fun s -> s.Trim ())
        |> Seq.filter (not << String.IsNullOrEmpty)
        |> Seq.map (fun s ->
            match s.Split ([| ':' |], 2) with
            | [| key; value |] -> key.Trim (), value.Trim ()
            | _ -> failwithf "Cannot parse metadata line %A" s)
        |> Map.ofSeq

let private readMetadata (fileName : string) documentNodes =
    let takeUntil cond seq =
        let found = ref false
        seq
        |> Seq.takeWhile (fun i ->
            let ret = not !found
            found := not <| cond i
            ret)

    let isBlockOfType tag (b : EnumeratorEntry) = (not << isNull) b.Block && b.Block.Tag = tag
    let isHorizontalRuler = isBlockOfType BlockTag.ThematicBreak
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
        Url = sprintf "/posts/%s.html" fileName
        Date = date
        Title = getMeta "title" ""
        Description = getMeta "description" ""
    }

let private getParseSettings () =
    let settings = CommonMarkSettings.Default.Clone ()
    settings.OutputDelegate <- fun doc output settings -> Formatter(output, settings).WriteDocument doc
    settings

let processMetadata filePath (reader : TextReader) =
    let document = CommonMarkConverter.Parse reader
    readMetadata (Path.GetFileNameWithoutExtension filePath) <| document.AsEnumerable ()

let processReader filePath (reader : TextReader)  =
    use target = new StringWriter ()
    let document = CommonMarkConverter.Parse reader
    let metadata = readMetadata (Path.GetFileNameWithoutExtension filePath) <| document.AsEnumerable ()
    let settings = getParseSettings ()

    CommonMarkConverter.ProcessStage3 (document, target, settings)

    {
        Meta = metadata
        HtmlContent = target.ToString ()
    }

let render (filePath : string): Async<PostModel> =
    async {
        do! Async.SwitchToThreadPool ()

        use reader = new StreamReader (filePath, Encoding.UTF8)
        return processReader filePath reader
    }
