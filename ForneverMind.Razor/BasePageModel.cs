using ForneverMind.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForneverMind.Razor;

public class BasePageModel : PageModel
{
    public LanguageLinks Links => new(
        english: new LanguageLink(isActive: false, link: ProduceLink(PageContext, "en")),
        russian: new LanguageLink(isActive: false, link: ProduceLink(PageContext, "ru"))
    );

    private static string ProduceLink(PageContext context, string newLanguage)
    {
        var currentUrl = context.HttpContext.Request.Path.Value;
        return $"{newLanguage}/{currentUrl?.Substring("/en".Length)}";
    }
}
