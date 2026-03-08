// SPDX-FileCopyrightText: 2025-2026 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

using ForneverMind.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForneverMind.Razor;

public class BasePageModel : PageModel
{
    public bool LinksActive { get; set; }

    private LanguageLinks? _links;

    public LanguageLinks Links
    {
        get => _links ?? new LanguageLinks(
            english: new LanguageLink(isActive: LinksActive, link: ProduceLink(PageContext, "en")),
            russian: new LanguageLink(isActive: LinksActive, link: ProduceLink(PageContext, "ru"))
        );
        set => _links = value;
    }

    private static string ProduceLink(PageContext context, string newLanguage)
    {
        var currentUrl = context.HttpContext.Request.Path.Value;
        var remainder = currentUrl?.Substring("/en".Length).TrimStart('/');
        return $"/{newLanguage}/{remainder}";
    }
}
