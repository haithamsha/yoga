using System.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using yoga.Data;
using yoga.Models;

namespace yoga.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly YogaAppDbContext _db;
    public HomeController(ILogger<HomeController> logger,  YogaAppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    public IActionResult Index()
    {
        // var platforms = _db.Platforms.ToList();
        // return View(platforms);
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Policy()
    {
        return View();
    }

    public IActionResult PrivacyAr()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    public IActionResult AboutAr()
    {
        return View();
    }

    public IActionResult History()
    {
        return View();
    }
    public IActionResult HistoryAr()
    {
        return View();
    }

    public IActionResult News()
    {
        return View();
    }

    public IActionResult NewsAr()
    {
        return View();
    }

    public IActionResult Downloads()
    {
        return View();
    }

    public IActionResult DownloadsAr()
    {
        return View();
    }

    public IActionResult FAQS()
    {
        return View();
    }

    public IActionResult FAQSAr()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

     public IActionResult ContactAr()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(string email)
    {
        return View();
    }

    [HttpPost]
    public IActionResult CultureManagement(string culture, string returnUrl)
    {
        Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, 
        CookieRequestCultureProvider.MakeCookieValue(
            new RequestCulture(culture)
        ), new CookieOptions{ Expires = DateTimeOffset.Now.AddDays(30)});

        return LocalRedirect(returnUrl);
    }

    public IActionResult IndexAr()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
