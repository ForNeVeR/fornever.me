module ForneverMind.Config

open System
open System.IO

let applicationPath = AppDomain.CurrentDomain.BaseDirectory
let viewsDirectory = Path.Combine (applicationPath, "Views")
