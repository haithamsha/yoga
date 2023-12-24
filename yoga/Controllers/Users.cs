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
        public Users(UserManager<AppUser> userManager, YogaAppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {

            var _users  = _userManager.Users.ToList();
            string userId = _userManager.GetUserId(User);

            if(User.IsInRole("Admin"))
            {
                _users = _userManager.Users
            .Where(u=>u.NationalId == "11")
            .ToList();
            }
            else {
                _users = _userManager.Users
            .Where(u=>u.NationalId == "11" && u.Id == userId)
            .ToList();
            }
            
            return View(_users);
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
            ModelState.Remove("Roles");
            var newUser = new AppUser { UserName = user.Email, Email = user.Email, PhoneNumber = user.Phone, 
                Discriminator = "Default", FirstName=user.FirstName, LastName = user.Email, NationalId="11", MiddleName = "dd",
                UserImage = "ii", EmailConfirmed = true, NationalIdImage = "nn" };

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
                    return View(user);
                }

                var result = await _userManager.CreateAsync(newUser, user.Password);
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






    }
}