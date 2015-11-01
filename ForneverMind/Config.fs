module ForneverMind.Config

open System
open System.IO

let applicationPath = AppDomain.CurrentDomain.BaseDirectory
let lessDirectory = Path.Combine (applicationPath, "less")
let viewsDirectory = Path.Combine (applicationPath, "Views")
