using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_barkery1.data;
using Web_barkery1.Helpers;

namespace Web_barkery1.Controllers;

public class SeoController(BakeryDbContext db, IConfiguration config) : Controller
{
    private string SiteUrl => (config["Seo:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}").TrimEnd('/');

    [HttpGet("/san-pham/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        var match = System.Text.RegularExpressions.Regex.Match(slug, @"-(\d+)$");
        if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id)) return NotFound();
        var product = await db.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();
        var canonical = SeoUrl.Product(product);
        if (!string.Equals(Request.Path.Value, canonical, StringComparison.Ordinal))
            return RedirectPermanent(canonical);
        ViewData["CanonicalUrl"] = SiteUrl + canonical;
        return View("~/Views/Shop/product-detail.cshtml", product);
    }

    [HttpGet("/san-pham")]
    public IActionResult Catalog()
    {
        ViewData["CanonicalUrl"] = SiteUrl + "/san-pham";
        return View("~/Views/Shop/products.cshtml");
    }

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Sitemap()
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urls = new List<string> { "/", "/san-pham" };
        urls.AddRange((await db.Products.AsNoTracking().OrderBy(x => x.Id).ToListAsync()).Select(SeoUrl.Product));
        var xml = new XDocument(new XElement(ns + "urlset", urls.Select(x =>
            new XElement(ns + "url", new XElement(ns + "loc", SiteUrl + x)))));
        return Content(xml.ToString(), "application/xml; charset=utf-8");
    }

    [HttpGet("/robots.txt")]
    public IActionResult Robots() => Content(
        "User-agent: *\nDisallow: /api/\nDisallow: /Account/\nDisallow: /Owner/\nDisallow: /Staff/\nDisallow: /pages/dashboard\nDisallow: /pages/account.html\nDisallow: /pages/cart.html\nDisallow: /pages/checkout.html\nDisallow: /pages/login.html\nDisallow: /pages/register.html\nSitemap: " + SiteUrl + "/sitemap.xml\n",
        "text/plain; charset=utf-8");
}
