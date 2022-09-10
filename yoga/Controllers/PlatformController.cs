using Microsoft.AspNetCore.Mvc;
using yoga.Data;

namespace yoga.Controllers
{
    public class PlatformController: Controller
    {
        private readonly ILogger<PlatformController> _logger;
        private readonly YogaAppDbContext _db;
        public PlatformController(ILogger<PlatformController> logger,  YogaAppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var platforms = _db.Platforms.ToList();
            return View(platforms);
        }

        public IActionResult IndexAr()
        {
            var platforms = _db.Platforms.ToList();
            return View(platforms);
        }

        public IActionResult Detail(int id)
        {
            var platform = _db.Platforms.Find(id);
            return View(platform);
        }
        public IActionResult DetailAr(int id)
        {
            var platform = _db.Platforms.Find(id);
            return View(platform);
        }


    }
}