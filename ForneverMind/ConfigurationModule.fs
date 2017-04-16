namespace ForneverMind

open System.IO

open Microsoft.AspNetCore.Hosting

type ConfigurationModule  (app : IHostingEnvironment) =
    // TODO[F]: Take this path from JSON configuration
    let baseUrl = "http://localhost:5000/" //  ConfigurationManager.AppSettings.["BaseUrl"]

    let root = app.ContentRootPath
    let postsPath = Path.Combine(root, "posts")
    let viewsPath = Path.Combine(root, "views")

    member __.BaseUrl = baseUrl
    member __.PostsPath = postsPath
    member __.ViewsPath = viewsPath
