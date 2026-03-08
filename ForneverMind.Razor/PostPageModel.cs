// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

using ForneverMind.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForneverMind.Razor;

public class PostPageModel : BasePageModel
{
    private readonly IPostRenderer _renderer;

    public PostPageModel(IPostRenderer renderer)
    {
        _renderer = renderer;
    }

    public PostModel Post { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string name)
    {
        if (!name.EndsWith(".html"))
            return NotFound();

        var baseName = Path.GetFileNameWithoutExtension(name);
        var language = HttpContext.Request.Path.Value!.StartsWith("/ru/") ? "ru" : "en";

        var filePath = Path.Combine(_renderer.PostsPath, language, baseName + ".md");
        var fullPostsPath = Path.GetFullPath(_renderer.PostsPath);
        var fullFilePath = Path.GetFullPath(filePath);
        if (!fullFilePath.StartsWith(fullPostsPath))
            return NotFound();

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        Post = await _renderer.Render(filePath);

        // Set Last-Modified header based on post file modification time
        var lastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
        Response.GetTypedHeaders().LastModified = new DateTimeOffset(lastModified);

        // Compute per-post language links based on file existence
        var enPath = Path.Combine(_renderer.PostsPath, "en", baseName + ".md");
        var ruPath = Path.Combine(_renderer.PostsPath, "ru", baseName + ".md");
        var enExists = System.IO.File.Exists(enPath);
        var ruExists = System.IO.File.Exists(ruPath);

        Links = new LanguageLinks(
            english: new LanguageLink(isActive: enExists, link: $"/en/posts/{name}"),
            russian: new LanguageLink(isActive: ruExists, link: $"/ru/posts/{name}")
        );

        return Page();
    }
}
