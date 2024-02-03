using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yoga.Data;
using yoga.Models;

namespace yoga.Controllers
{
    public class NotificationsController: Controller
    {
        private readonly YogaAppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public NotificationsController(YogaAppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);

            IQueryable<Notification> result = _db.Notification
            .Include("AppUser");

            if(!User.IsInRole("Admin"))
            {
                result = result
                .Include("AppUser")
                .Include("AdminUser")
                .Where(n=>n.AppUser.Id == userId);
            }
            else {
                result = result
                .Include("AdminUser")
                .Include("AppUser");
            }
            

            return View(await result.OrderByDescending(d=>d.CreationDate).ToListAsync());
        }
    }
}