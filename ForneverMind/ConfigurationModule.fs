namespace ForneverMind

open System.IO

open Microsoft.AspNetCore.Hosting

type ConfigurationModule  (app : IHostingEnvironment) =
    let root = app.ContentRootPath
    let postsPath = Path.Combine(root, "posts")
    let viewsPath = Path.Combine(root, "views")

    member __.PostsPath = postsPath
    member __.ViewsPath = viewsPath
