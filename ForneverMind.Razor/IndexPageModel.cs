// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

using ForneverMind.Core;
using Microsoft.AspNetCore.Http;

namespace ForneverMind.Razor;

public class IndexPageModel : BasePageModel
{
    private const int IndexPostCount = 20;

    private readonly IPostsProvider _postsProvider;

    public PostMetadata[] Posts { get; set; } = Array.Empty<PostMetadata>();

    public IndexPageModel(IPostsProvider postsProvider)
    {
        _postsProvider = postsProvider;
    }

    public void OnGet()
    {
        var language = HttpContext.Request.Path.Value!.StartsWith("/ru/") ? "ru" : "en";
        Posts = _postsProvider.AllPosts(language).Take(IndexPostCount).ToArray();
        if (Posts.Length > 0)
        {
            Response.GetTypedHeaders().LastModified = new DateTimeOffset(Posts[0].Date);
        }
    }
}
