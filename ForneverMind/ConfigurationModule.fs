namespace ForneverMind

open System.IO

open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

type ConfigurationModule(app : IHostingEnvironment, config : IConfigurationRoot) =
    let root = app.ContentRootPath
    let postsPath = Path.Combine(root, "posts")
    let viewsPath = Path.Combine(root, "views")

    let baseUrl = config.["baseUrl"]

    member __.PostsPath = postsPath
    member __.ViewsPath = viewsPath
    member __.BaseUrl = baseUrl
