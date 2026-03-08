// SPDX-FileCopyrightText: 2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

using ForneverMind.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TruePath.SystemIo;

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

        var filePath = _renderer.PostsPath / language / (baseName + ".md");
        if (!FileSystem.IsPathInsideDirectory(_renderer.PostsPath, filePath))
            return NotFound();

        if (!filePath.ExistsFile())
            return NotFound();

        Post = await _renderer.Render(filePath);

        // Set Last-Modified header based on post file modification time
        var lastModified = filePath.GetLastWriteTimeUtc();
        Response.GetTypedHeaders().LastModified = new DateTimeOffset(lastModified);

        // Compute per-post language links based on file existence
        var enPath = _renderer.PostsPath / "en" / (baseName + ".md");
        var ruPath = _renderer.PostsPath / "ru" / (baseName + ".md");
        var enExists = enPath.ExistsFile();
        var ruExists = ruPath.ExistsFile();

        Links = new LanguageLinks(
            english: new LanguageLink(isActive: enExists, link: $"/en/posts/{name}"),
            russian: new LanguageLink(isActive: ruExists, link: $"/ru/posts/{name}")
        );

        return Page();
    }
}
