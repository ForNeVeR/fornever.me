// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

namespace ForneverMind

open System
open System.Collections.Generic
open System.IO

open ForneverMind.Core
open RazorLight

type TemplatingModule (config: ConfigurationModule) =
    let razor =
        RazorLightEngineBuilder()
            .UseFileSystemProject(config.ViewsPath)
            .Build()

    let templateName language name = sprintf "%s/%s.cshtml" language name
    let templatePath language name = Path.Combine(config.ViewsPath, templateName language name)
    let layoutPath language = templatePath language "_Layout"

    let prepareViewBag (links : LanguageLinks) =
        let viewBag = Dynamic.ExpandoObject()
        let dictionary = viewBag :> IDictionary<string, obj>
        dictionary.["Links"] <- links
        viewBag

    member __.LastModificationDate (language : string) (name : string) : DateTime =
        let templateModificationDate = File.GetLastWriteTimeUtc <| templatePath language name
        let layoutModificationDate = File.GetLastWriteTimeUtc <| layoutPath language
        max templateModificationDate layoutModificationDate

    member __.Render<'a> (language : string)
                         (name : string)
                         (model : 'a option)
                         (links : LanguageLinks) : Async<string> =
        async {
            do! Async.SwitchToThreadPool ()
            let value =
                match model with
                | Some(v) -> v
                | None -> Unchecked.defaultof<'a>
            let viewBag = prepareViewBag links
            let! result = Async.AwaitTask <| razor.CompileRenderAsync(templateName language name, value, viewBag)
            return result
        }
