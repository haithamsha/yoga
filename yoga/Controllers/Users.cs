using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using yoga.Data;
using yoga.Models;
using yoga.ViewModels;

namespace yoga.Controllers
{

    public class Users:Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly YogaAppDbContext _db;
        private readonly DateTime EndDate;

        public Users(UserManager<AppUser> userManager, YogaAppDbContext db)
        {
            _userManager = userManager;
            _db = db;
            EndDate = new DateTime(2222, 06, 06);
        }

        [Authorize]
        public IActionResult Index()
        {

            //var _users  = new List<UserVM>();
            string userId = _userManager.GetUserId(User);

            if(User.IsInRole("Admin"))
            {
                var _users = _db.Users
                .Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_db.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .Where(u => u.ur.u.NationalId ==  "11")
                .ToList()
                .GroupBy(uv => new { uv.ur.u.UserName, uv.ur.u.Email, uv.ur.u.Id, uv.ur.u.LockoutEnd, uv.ur.u.FirstName, uv.ur.u.LastName }).Select(r => new UserVM()
                {
                    FirstName = $"{r.Key.FirstName} {r.Key?.LastName}",
                    Email = r.Key.Email,
                    RoleNames = string.Join(",", r.Select(c => c.r.Name).ToArray()),
                    Id = r.Key.Id,
                    Status = r.Key.LockoutEnd == null ? true : false
                })
                .ToList();
                
                return View(_users);
            }
            else 
            {
                var _users = _db.Users
                .Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_db.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .Where(u => u.ur.u.NationalId ==  "11" && u.ur.u.Id == userId)
                .ToList()
                .GroupBy(uv => new { uv.ur.u.UserName, uv.ur.u.Email , uv.ur.u.Id, uv.ur.u.LockoutEnd, uv.ur.u.FirstName, uv.ur.u.LastName }).Select(r => new UserVM()
                {
                    FirstName = $"{r.Key.FirstName} {r.Key?.LastName}",
                    Email = r.Key.Email,
                    RoleNames = string.Join(",", r.Select(c => c.r.Name).ToArray()),
                    Id = r.Key.Id,
                    Status = r.Key.LockoutEnd == null ? false : true,

                })
                .ToList();
                
                return View(_users);
            }
            
        }

        private List<SelectListItem> GetRoles()
            {
                var roles = _db.Roles
                .Where(r=> r.Name == "Admin" || r.Name == "Manager")
                .Select(r=>new SelectListItem() {
                            Value = r.Name.ToString(),
                            Text = r.Name})
                .ToList();
                return roles;
                    
            }

        [Authorize]
        public IActionResult Create()
        {   
            var roles = GetRoles();
            var vm = new RegisterModel();
            vm.Roles = roles;
            
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterModel user)
        {
            ModelState.Remove("ReturnUrl");
            ModelState.Remove("NationalId");
            ModelState.Remove("MiddleName");
            ModelState.Remove("LastName");
            ModelState.Remove("Image");
            ModelState.Remove("NationalIdImage");
            ModelState.Remove("Counries");
            ModelState.Remove("Cities");
            ModelState.Remove("Roles");

            // get default city
            var city = _db.Cities.Find(1);
            var newUser = new AppUser { UserName = user.Email, Email = user.Email, PhoneNumber = user.Phone, 
                Discriminator = "Default", FirstName=user.FirstName, LastName = user.Email, NationalId="11", MiddleName = "dd",
                UserImage = "ii", EmailConfirmed = true, NationalIdImage = "nn", City = city };

            var roles = GetRoles();
            // Validate if user exists before
                string validationError = "";

                if(_db.Users.Where(u=>u.Email == user.Email).ToList().Count >=1)
                {
                    validationError += " This Email used before!,";
                }
                if(_db.Users.Where(u=>u.PhoneNumber == user.Phone).ToList().Count >=1)
                {
                    validationError += " This Phone used before!,";
                }

                if(!string.IsNullOrEmpty(validationError))
                {
                    ModelState.AddModelError("", validationError);
                    
                    user.Roles = roles;
                    return View(user);
                }

                var result = new IdentityResult();
                try
                {
                    result = await _userManager.CreateAsync(newUser, user.Password);
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    user.Roles = roles;
                    return View(user);
                }
                if(result.Succeeded)
                {
                    // Add user to role
                    await _userManager.AddToRoleAsync(newUser, user.RoleId);
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                user.Roles = roles;
                return View(user);
        }

        [Authorize]
        public IActionResult Edit(string? Id = "")
        {
           
            string userId = "";
            if(!string.IsNullOrEmpty(Id))
            {
                userId = Id;
            }
            
            var user = _db.Users.Where(u=>u.Id == userId)
            .Select(u => new RegisterModel
            {
                NationalId = u.NationalId,
                Email = u.Email,
                Phone = u.PhoneNumber,
                FirstName = u.UserName
            })
            .FirstOrDefault();

            if(user == null) return NotFound();
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(RegisterModel user, string Id)
        {
             ModelState.Remove("RoleId");
            ModelState.Remove("Cities");
            ModelState.Remove("Roles");
            ModelState.Remove("ReturnUrl");
            ModelState.Remove("NationalId");
            ModelState.Remove("FirstName");
            ModelState.Remove("MiddleName");
            ModelState.Remove("LastName");
            ModelState.Remove("Image");
            ModelState.Remove("Counries");
            ModelState.Remove("NationalIdImage");
             ModelState.Remove("Password");
              ModelState.Remove("ConfirmPassword");
            var userData = _db.Users.Find(Id);
            if(userData == null) return NotFound();
            // Validate if user exists before
            string validationError = "";

            if( userData.Email == user.Email &&userData.Id != Id)
            {
                validationError += " This Email used before!,";
            }
            if(userData.PhoneNumber == user.Phone &&userData.Id != Id)
            {
                validationError += " This Phone used before!,";
            }
            
            userData.Email = user.Email;
            userData.UserName = user.Email;
            userData.PhoneNumber = user.Phone;
            _db.Users.Update(userData);
            _db.SaveChanges();

            return View(user);
        }

        public async Task<IActionResult> LockUser(string email, DateTime? endDate)
        {
            if (endDate == null)
                endDate = EndDate;

            var user = await _userManager.FindByEmailAsync(email);

            var lockUserTask = await _userManager.SetLockoutEnabledAsync(user, true);

            var lockDateTask =await _userManager.SetLockoutEndDateAsync(user, endDate);

            if(lockDateTask.Succeeded) return RedirectToAction("Index", "Users");

            ViewBag.Error = $"Error while Update user data {lockDateTask.Errors?.FirstOrDefault()?.Description}";

            return RedirectToAction("Index", "Users");
            
        }

        public async Task<IActionResult> UnLockUser(string email, DateTime? endDate)
        {
            if (endDate == null)
                endDate = EndDate;

            var user = await _userManager.FindByEmailAsync(email);

            var lockUserTask = await _userManager.SetLockoutEnabledAsync(user, false);

            var lockDateTask =await _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));

            if(lockDateTask.Succeeded) return RedirectToAction("Index", "Users");

            ViewBag.Error = $"Error while Update user data {lockDateTask.Errors?.FirstOrDefault()?.Description}";

            return RedirectToAction("Index", "Users");
            
        }





    }
}