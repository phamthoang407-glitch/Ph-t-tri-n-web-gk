using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Web_barkery1.Models;

namespace Web_barkery1.Helpers;

public static class SeoUrl
{
    public static string Slug(string? input)
    {
        var source = (input ?? string.Empty).Trim().ToLowerInvariant().Replace('đ', 'd');
        var decomposed = source.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        var slug = Regex.Replace(sb.ToString(), "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "banh" : slug;
    }

    // ID ổn định khi có nhiều loại bánh trùng tên; đổi tên sẽ có redirect 301.
    public static string Product(Product product) => $"/san-pham/{Slug(product.Name)}-{product.Id}";
}
