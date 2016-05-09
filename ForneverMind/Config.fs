module ForneverMind.Config

open System
open System.Configuration
open System.IO

let baseUrl = ConfigurationManager.AppSettings.["BaseUrl"]

let applicationPath = AppDomain.CurrentDomain.BaseDirectory
let lessDirectory = Path.Combine (applicationPath, "less")
let postsDirectory = Path.Combine (applicationPath, "posts")
let viewsDirectory = Path.Combine (applicationPath, "views")
