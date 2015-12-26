module ForneverMind.Less

open System.IO
open System.Text

open Arachne.Http
open Freya.Core
open Freya.Machine
open Freya.Machine.Extensions.Http
open dotless.Core
open dotless.Core.configuration

let private mainCss =
    let path = Config.lessDirectory
    let config = DotlessConfiguration()
    let locator = ContainerFactory().GetContainer(config)
    let engine = (locator.GetService (typeof<ILessEngine>)) :?> ILessEngine
    engine.CurrentDirectory <- path

    let fileName = Path.Combine (path, "main.less")
    let content = File.ReadAllText fileName
    let result = engine.TransformToCss (content, fileName)
    result

let private representation =
    {
        Description =
            {
                Charset = Some Charset.Utf8
                Encodings = None
                MediaType = Some MediaType.Css
                Languages = None
            }
        Data = Encoding.UTF8.GetBytes mainCss
    } |> Freya.init

let private lastModifiedDate =
    Directory.GetFileSystemEntries Config.lessDirectory
    |> Seq.map File.GetLastWriteTimeUtc
    |> Seq.max

let main =
    freyaMachine {
        including Common.machine
        methodsSupported Common.get
        lastModified (Common.initLastModified lastModifiedDate)
        handleOk (fun _ -> representation)
    } |> FreyaMachine.toPipeline
