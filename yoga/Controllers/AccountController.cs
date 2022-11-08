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
    [Authorize]
    public class AccountController: Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly YogaAppDbContext _db;
        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger, YogaAppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [Authorize]
        public IActionResult UserProfile()
        {
            var userSetting = new UserSetting();

            var _userSubscribtions = new UserSubscribtions();

            // Check for user subscriptions
            string userId =  _userManager.GetUserId(User);

            //var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
            var techLics = _db.TechearMemberShips.Include("AppUser").Where(m=>m.AppUser.Id == userId)
            .SingleOrDefault();

            if(techLics != null)
            {
                userSetting.User_Subscribtions.HasTeacherLic = true;
                var tlic = _db.TechearMemberShips.Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.TeacherLic.ExpireDate = tlic.ExpireDate.HasValue ?   tlic.ExpireDate.Value.ToShortDateString() : "";
                userSetting.TeacherLic.FinalApprove = tlic.FinalApprove;
                userSetting.TeacherLic.Status = tlic.Status;
                userSetting.TeacherLic.PassExam = tlic.PassExam;
                userSetting.TeacherLic.TakeExam = tlic.TakeExam;
                userSetting.TeacherLic.PayFees = tlic.PayFees;
                userSetting.TeacherLic.PayExamFees = tlic.PayExamFees;
                userSetting.TeacherLic.ReceiptCopy = tlic.ReceiptCopy;
                userSetting.TeacherLic.ReceiptCopyLic = tlic.ReceiptCopyLic;

            } 

            var mem = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();

            if(mem != null)
            {
                userSetting.User_Subscribtions.HasMemberShip = true;
                var memC = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.MemshipCard.Active = memC.Active;
                userSetting.MemshipCard.ExpireDate = memC.ExpireDate.HasValue ? memC.ExpireDate.Value.ToShortDateString() : "";
                userSetting.MemshipCard.Status = memC.Status;
            }                

            return View(userSetting);
        }

        private List<SelectListItem> GetCountries()
        {
            var countries =  _db.Country
                        .Select(r=>new SelectListItem() {
                            Value = r.CountryId.ToString(),
                            Text = r.EnName
                        })
                        .ToList();
            return countries;
            
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            var countries = GetCountries();
            var vm = new RegisterModel();
            vm.Counries = countries;
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel Input, IFormFile Image, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            Input.ReturnUrl = returnUrl;
            ModelState.Remove("ReturnUrl");
            ModelState.Remove("Image");
            ModelState.Remove("Counries");
            if (ModelState.IsValid)
            {
                    // Upload image
                    string fileName = "";
                    if(Image != null && Image.Length > 0)
                    {
                        fileName = Path.GetFileName(Image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName);
                        using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileSrteam);
                        }
                    }
                    else {
                        ModelState.AddModelError("", "Image required for Membership and license!");
                        var countries_img = GetCountries();
                        Input.Counries = countries_img;
                        return View(Input);
                    }

                var country = _db.Country.Find(Input.CountryId);

                var user = new AppUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.Phone, NationalId = Input.NationalId, 
                MiddleName = Input.MiddleName, Discriminator = "Default", LastName = Input.LastName, 
                FirstName = Input.FirstName, UserImage = fileName, Country = country};

                // Validate if user exists before
                string validationError = "";

                if(_db.Users.Where(u=>u.Email == user.Email).ToList().Count >=1)
                {
                    validationError += " This Email used before!,";
                }
                if(_db.Users.Where(u=>u.PhoneNumber == user.PhoneNumber).ToList().Count >=1)
                {
                    validationError += " This Phone used before!,";
                }
                if(_db.Users.Where(u=>u.NationalId == user.NationalId).ToList().Count >=1)
                {
                    validationError += " This National Id used before!";
                }

                if(!string.IsNullOrEmpty(validationError))
                {
                    var _countries = GetCountries();
                    Input.Counries = _countries;
                    ModelState.AddModelError("", validationError);
                    return View(Input);
                }


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Send Confirmation Email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    string confirmationLink = Url.Action(nameof(ConfirmEmail), 
                    "Account", new { token, email = user.Email }, Request.Scheme);

                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    
                    string content = $"Please Confirm Your Account By <a href='{confirmationLink}'>Clicking Here</a>";
                    var emailMessage = new EmailMessage
                    {
                        ToEmailAddresses = new List<string> {user.Email},
                        Subject = "SAUDI YOGA COMMITTEE - Confirmation Code",
                        Body = content
                    };
                    
                    _emailSender.SendEmailAsync(emailMessage);

                    //await _signInManager.SignInAsync(user, isPersistent: false);

                    
                    return RedirectToAction(nameof(SuccessRegistration));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            var countries = GetCountries();
            Input.Counries = countries;
            return View(Input);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SuccessRegistration()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel Input)
        {
            ModelState.Remove("ErrorMessage");
        
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, 
                Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("UserProfile");
                }
                else
                {
                    var user = _userManager.FindByEmailAsync(Input.Email).Result;

                    if(user != null) 
                    {
                        if(await _userManager.IsEmailConfirmedAsync(user) == false)
                        {
                            ModelState.AddModelError(string.Empty, "Your Email Not Confirmed Yet");
                            return View();
                        }
                    }
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }
 
            // If we got this far, something failed, redisplay form
            return View();
        }

        public IActionResult Settings()
        {
            var userSetting = new UserSetting();

            var _userSubscribtions = new UserSubscribtions();

            // Check for user subscriptions
            string userId =  _userManager.GetUserId(User);

            //var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
            var techLics = _db.TechearMemberShips.Include("AppUser").Where(m=>m.AppUser.Id == userId)
            .SingleOrDefault();

            if(techLics != null)
            {
                userSetting.User_Subscribtions.HasTeacherLic = true;
                var tlic = _db.TechearMemberShips.Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.TeacherLic.ExpireDate = tlic.ExpireDate.HasValue ?   tlic.ExpireDate.Value.ToShortDateString() : "";
                userSetting.TeacherLic.FinalApprove = tlic.FinalApprove;
                userSetting.TeacherLic.Status = tlic.Status;
                userSetting.TeacherLic.Serial = tlic.SerialNumber;

            } 

            var mem = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();

            if(mem != null)
            {
                userSetting.User_Subscribtions.HasMemberShip = true;
                var memC = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.MemshipCard.Active = memC.Active;
                userSetting.MemshipCard.ExpireDate = memC.ExpireDate.HasValue ? memC.ExpireDate.Value.ToShortDateString() : "";
                userSetting.MemshipCard.Status = memC.Status;
                userSetting.MemshipCard.Serial = memC.SerialNumber;
            }                

            return View(userSetting);
        }

        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }
    }
}