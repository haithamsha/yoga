using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yoga.Data;
using yoga.Migrations;
using yoga.ViewModels;

namespace yoga.Controllers
{
    public class WFHistoryController: Controller
    {
        private readonly YogaAppDbContext _db;
        public WFHistoryController(YogaAppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var wfHistoryList = _db.WFHistory
            .Include(h => h.AppUser)
            .Select(h=> new WFHistoryVM {
                CreationDate = h.CreationDate,
                Description = h.Description,
                HistoryId = h.HistoryId,
                HistoryType = h.WFHistoryType.ToString(),
                ModuleName  = h.ModuleName,
                RecordId = h.RecordId,
                UserName = h.AppUser.LastName
            })
            .ToList();
            return View(wfHistoryList);
        }

        public IActionResult Detail(int id)
        {
            var wfHistory = _db.WFHistory
            .Include("User")
            .Where(h=>h.HistoryId == id)
            .ToList();
            return View();
        }

    }
}