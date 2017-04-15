module ForneverMind.Config

open System
//open System.Configuration
open System.IO

// TODO[F]: Take this path from JSON configuration
let baseUrl = "http://localhost:5000/" //  ConfigurationManager.AppSettings.["BaseUrl"]

// TODO[F]: This should be a web app directory
let applicationPath = "." // AppDomain.CurrentDomain.BaseDirectory
let postsDirectory = Path.Combine (applicationPath, "posts")
let viewsDirectory = Path.Combine (applicationPath, "views")
