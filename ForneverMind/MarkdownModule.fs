namespace ForneverMind

open System
open System.Globalization
open System.IO
open System.Text

open CommonMark
open CommonMark.Formatters
open CommonMark.Syntax
open Microsoft.AspNetCore.NodeServices

open ForneverMind.Models

type MarkdownModule(highlight : CodeHighlightModule) =
    let markdownFormatter (highlight: CodeHighlightModule) target settings =
        let skippedCode = ref false
        let skippedRuler = ref false
        { new HtmlFormatter (target, settings) with
              override this.WriteBlock (block, isOpening, isClosing, ignoreChildNodes) =
                  let getCodeLanguage () =
                      Option.ofObj block.FencedCodeData
                      |> Option.map (fun data -> data.Info)

                  let skipCode = not !skippedCode && block.Tag = BlockTag.IndentedCode
                  let skipRuler = not !skippedRuler && block.Tag = BlockTag.ThematicBreak
                  match skipCode, skipRuler with
                  | (true, _) -> skippedCode := true; ()
                  | (_, true) -> skippedCode := true; skippedRuler := true; ()
                  | _ ->
                      match block.Tag with
                      | BlockTag.FencedCode | BlockTag.IndentedCode ->
                          let language = getCodeLanguage()
                          let code = block.StringContent.ToString()

                          ignoreChildNodes <- true
                          this.EnsureNewLine ()
                          this.Write "<pre><code class=\"hljs\">"
                          let code = Async.RunSynchronously <| highlight.Highlight(language, code)
                          this.Write code
                          this.WriteLine "</code></pre>"
                      | _ ->
                          ignoreChildNodes <- false
                          base.WriteBlock (block, isOpening, isClosing, ref ignoreChildNodes) }

    let getMetadata (block : EnumeratorEntry option) =
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

    let readMetadata language (fileName : string) documentNodes =
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
            Url = sprintf "/%s/posts/%s.html" language fileName
            Date = date
            Title = getMeta "title" ""
            Description = getMeta "description" ""
            CommentUrl = getMeta "commentUrl" ""
        }

    let getParseSettings () =
        let settings = CommonMarkSettings.Default.Clone ()
        settings.OutputDelegate <- fun doc output settings ->
            let formatter = markdownFormatter highlight output settings
            formatter.WriteDocument doc
        settings

    let getLanguageAndName (path : string) =
        let language = Path.GetFileName (Path.GetDirectoryName path)
        let name = Path.GetFileNameWithoutExtension path
        language, name

    member __.ProcessMetadata(filePath : string, reader : TextReader): PostMetadata =
        let document = CommonMarkConverter.Parse reader
        let (language, name) = getLanguageAndName filePath
        readMetadata language name (document.AsEnumerable())

    member __.ProcessReader(filePath : string, reader : TextReader) =
        use target = new StringWriter ()
        let document = CommonMarkConverter.Parse reader
        let (language, name) = getLanguageAndName filePath
        let metadata = readMetadata language name (document.AsEnumerable())
        let settings = getParseSettings ()

        CommonMarkConverter.ProcessStage3 (document, target, settings)

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
