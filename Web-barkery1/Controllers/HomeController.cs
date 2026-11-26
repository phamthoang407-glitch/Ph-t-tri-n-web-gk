using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_barkery1.data;
using System.Diagnostics;
using Web_barkery1.Models;

namespace Web_barkery1.Controllers
{
    public class HomeController : Controller
    {
        private readonly BakeryDbContext _db;
        public HomeController(BakeryDbContext db) { _db = db; }
        public IActionResult Index() => Redirect("/");

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
