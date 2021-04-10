module ForneverMind.Markdown.MarkdownUtils

open System.IO

open Markdig.Syntax

let extractCode(block: LeafBlock): string =
    use writer = new StringWriter()
    let lines = block.Lines.Lines
    for i in 0 .. block.Lines.Count - 1 do
        writer.WriteLine(lines.[i].Slice)
    writer.ToString()
