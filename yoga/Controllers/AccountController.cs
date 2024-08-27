using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Security.Claims;
using yoga.Data;
using yoga.Models;
using yoga.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public IActionResult Index()
        {
            string userId =  _userManager.GetUserId(User);

            IEnumerable<TechearMemberShipTest> lics = _db.techearMemberShipTests
            .Include("TechearMemberShip")
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(t=>t.TechearMemberShip.AppUser.Id == userId);
            
            return View(lics);
        }

        [Authorize]
        public IActionResult UserProfile()
        {
            string userId =  _userManager.GetUserId(User);

            List<TechearMemberShipTest> techLics = _db.techearMemberShipTests
            .Include("TechearMemberShip")
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(t=>t.TechearMemberShip.AppUser.Id == userId).ToList();

            var userSetting = new UserSetting();
            userSetting.TechearMemberShipTest = techLics;

            var _userSubscribtions = new UserSubscribtions();


            if(techLics.Count > 0 )
            {
                userSetting.User_Subscribtions.HasTeacherLic = true;
                foreach (var item in techLics)
                {
                    item.TeachingType_string = GlobalHelpers.getTeachingType(item.TeachingType);
                }

            } 

            var mem = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();

            if(mem != null)
            {
                userSetting.User_Subscribtions.HasMemberShip = true;
                var memC = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.MemshipCard.Active = memC.Active;
                userSetting.MemshipCard.ExpireDate = memC.ExpireDate.HasValue ? memC.ExpireDate.Value.ToShortDateString() : "";
                userSetting.MemshipCard.Status = memC.Status;
                userSetting.MemshipCard.Id = memC.CardId;
            }                

            return View(userSetting);

        }

        [Authorize]
        public IActionResult UserProfileDetail(int? Id)
        {
            var userSetting = new UserSetting();

            var _userSubscribtions = new UserSubscribtions();

            // Check for user subscriptions
            string userId =  _userManager.GetUserId(User);

            //var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
            var techLics = _db.techearMemberShipTests
            .Include(t=>t.TechearMemberShip)
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(t=>t.TestId == Id)
            .SingleOrDefault();

            if(techLics != null)
            {
                userSetting.User_Subscribtions.HasTeacherLic = true;
                var tlic = _db.techearMemberShipTests
                .Include(t=>t.TechearMemberShip)
                .Where(m=>m.TestId == Id)
                .FirstOrDefault();

                userSetting.TeacherLic.ExpireDate = tlic.ExpireDate.HasValue ?   tlic.ExpireDate.Value.ToShortDateString() : "";
                userSetting.TeacherLic.IssueDate = tlic.ExpireDate.HasValue ?   tlic.ExpireDate.Value.AddYears(-1).ToShortDateString() : "";
                userSetting.TeacherLic.FinalApprove = tlic.FinalApprove;
                userSetting.TeacherLic.Status = tlic.Status;
                userSetting.TeacherLic.PassExam = tlic.PassExam;
                userSetting.TeacherLic.TakeExam = tlic.TakeExam;
                userSetting.TeacherLic.PayFees = tlic.PayFees;
                userSetting.TeacherLic.PayExamFees = tlic.PayExamFees;
                userSetting.TeacherLic.ReceiptCopy = tlic.ReceiptCopy;
                userSetting.TeacherLic.ReceiptCopyLic = tlic.ReceiptCopyLic;
                userSetting.TeacherLic.ExamLocation = tlic.ExamLocation;
                userSetting.TeacherLic.RejectReason = tlic.RejectReason;

            } 

            var mem = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();

            if(mem != null)
            {
                userSetting.User_Subscribtions.HasMemberShip = true;
                var memC = _db.MembershipCards.Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.MemshipCard.Active = memC.Active;
                userSetting.MemshipCard.ExpireDate = memC.ExpireDate.HasValue ? memC.ExpireDate.Value.ToShortDateString() : "";
                userSetting.MemshipCard.Status = memC.Status;
                userSetting.MemshipCard.Id = memC.CardId;
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

        private List<SelectListItem> GetCities(int countryId)
        {
            var cities =  _db.Cities
                        .Where(c=>c.CountryId == countryId)
                        .Select(r=>new SelectListItem() {
                            Value = r.CityId.ToString(),
                            Text = r.EnName
                        })
                        .ToList();
            return cities;
            
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            var countries = GetCountries();
            var cities = GetCities(1);
            var vm = new RegisterModel();
            vm.Counries = countries;
            vm.Cities = cities;
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel Input, IFormFile Image,
        IFormFile NationalIdImage, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            Input.ReturnUrl = returnUrl;
            ModelState.Remove("ReturnUrl");
            ModelState.Remove("Image");
            ModelState.Remove("NationalIdImage");
            ModelState.Remove("Counries");
            ModelState.Remove("Cities");
            ModelState.Remove("RoleId");
            ModelState.Remove("Roles");
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
                    else 
                    {
                        var _countries = GetCountries();
                        var _cities = GetCities(Input.CountryId);
                        Input.Counries = _countries;
                        Input.Cities = _cities;
                        ModelState.AddModelError("Image", "Image Required");
                        return View(Input);
                    }

                    // Upload NationalId image
                    string fileName_nationalId = "";
                    if(NationalIdImage != null && NationalIdImage.Length > 0)
                    {
                        fileName_nationalId = Path.GetFileName(NationalIdImage.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_nationalId);
                        using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                        {
                            await NationalIdImage.CopyToAsync(fileSrteam);
                        }
                    }
                    else {
                        var _countries = GetCountries();
                        Input.Counries = _countries;
                        var _cities = GetCities(Input.CountryId);
                        Input.Cities = _cities;
                        ModelState.AddModelError("NationalIdImage", "National Id required");
                        return View(Input);
                    }
                    

                var country = _db.Country.Find(Input.CountryId);
                var city = _db.Cities.Find(Input.CityId);

                var user = new AppUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.Phone, NationalId = Input.NationalId, 
                MiddleName = Input.MiddleName, Discriminator = "Default", LastName = Input.LastName, 
                FirstName = Input.FirstName, UserImage = fileName, 
                Country = country, City= city, NationalIdImage = fileName_nationalId};

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
                    var _cities = GetCities(Input.CountryId);
                    Input.Cities = _cities;
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
                    
                    _emailSender.SendEmailBySendGrid(emailMessage);

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
            var cities = GetCities(Input.CountryId);
            Input.Cities = cities;
            return View(Input);
        }

        [Authorize]
        public IActionResult EditUser(string? Id = "")
        {
            string userId = "";
            if(!string.IsNullOrEmpty(Id))
            {
                userId = Id;
            }
            else 
            {
                userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            var user = _db.Users.Where(u=>u.Id == userId)
            .Select(u => new RegisterModel
            {
                NationalId = u.NationalId,
                Email = u.Email,
                Phone = u.PhoneNumber,
                CountryId = u.Country.CountryId,
                MiddleName = u.MiddleName,
                LastName = u.LastName,
                Image = u.UserImage,
                NationalIdImage = u.NationalIdImage,
                FirstName = u.UserName,
                CityId = u.City.CityId
            })
            .FirstOrDefault();

            if(user == null) return NotFound();

            var countries = GetCountries();
            user.Counries = countries;
            var cities = GetCities(1);
            user.Cities = cities;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(RegisterModel Input, IFormFile Image, IFormFile NationalIdImage,
        string? Id = "")
        {
            string userId_nat = _userManager.GetUserId(User);
            var loggedUser = _db.Users.Where(u=>u.Id == userId_nat).FirstOrDefault();
            if(loggedUser.NationalId == "11")
            {
                ModelState.Remove("NationalId");
            }

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("ReturnUrl");
            ModelState.Remove("Image");
            ModelState.Remove("NationalIdImage");
            ModelState.Remove("Counries");
            ModelState.Remove("Cities");
            ModelState.Remove("RoleId");
            ModelState.Remove("Roles");
            if (ModelState.IsValid)
            {
                    string userId =  "";
                    if(!string.IsNullOrEmpty(Id))
                    {
                        userId = Id;
                    }
                    else 
                    {
                        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    var userData = _db.Users.Find(userId);

                    // Upload images
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

                    string fileName_nationalId = "";
                    if(NationalIdImage != null && NationalIdImage.Length > 0)
                    {
                        fileName_nationalId = Path.GetFileName(NationalIdImage.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_nationalId);
                        using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                        {
                            await NationalIdImage.CopyToAsync(fileSrteam);
                        }
                    }
                    

                var country = _db.Country.Find(Input.CountryId);
                var city = _db.Cities.Find(Input.CityId);

                string _userImage = string.IsNullOrEmpty(fileName) ? userData.UserImage : fileName;
                string _userImage_NationalId = string.IsNullOrEmpty(fileName_nationalId) ? userData.NationalIdImage : fileName_nationalId;

                var user = new AppUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.Phone, NationalId = Input.NationalId, 
                MiddleName = Input.MiddleName, Discriminator = "Default", LastName = Input.LastName, 
                FirstName = Input.FirstName, UserImage = _userImage, Country = country, NationalIdImage = _userImage_NationalId};

                if(userData.NationalId != "11")
                {
                    userData.NationalId = Input.NationalId;
                }
                else
                {
                    userData.NationalId = "11";
                }
                
                userData.Email = user.Email;
                userData.PhoneNumber = user.PhoneNumber;
                userData.Country = _db.Country.Find(Input.CountryId);
                userData.City = _db.Cities.Find(Input.CityId);
                userData.FirstName = user.FirstName;
                userData.MiddleName = user.MiddleName;
                userData.LastName = user.LastName;
                userData.UserImage = _userImage;
                userData.NationalIdImage = _userImage_NationalId;

                // Validate if user exists before
                string validationError = "";

                if( userData.Email == user.Email &&userData.Id != userId)
                {
                    validationError += " This Email used before!,";
                }
                if(userData.PhoneNumber == user.PhoneNumber &&userData.Id != userId)
                {
                    validationError += " This Phone used before!,";
                }
                if(userData.NationalId == user.NationalId &&userData.Id != userId)
                {
                    validationError += " This National Id used before!";
                }

                if(!string.IsNullOrEmpty(validationError))
                {
                    var _countries = GetCountries();
                    Input.Counries = _countries;
                    var cities = GetCities(Input.CountryId);
                    Input.Cities = cities;
                    ModelState.AddModelError("", validationError);
                    return View(Input);
                }


                _db.Users.Update(userData);
                
                _db.SaveChanges();

                if(!string.IsNullOrEmpty(Id)) return RedirectToAction("Users");
                
                return RedirectToAction("Settings");

                }

                // If we got this far, something failed, redisplay form
                var countries = GetCountries();
                Input.Counries = countries;
                var _cities = GetCities(Input.CountryId);
                Input.Cities = _cities;
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


        [Authorize]
        public IActionResult ChangePassword(string? Id = "")
        {
            string userId = "";
            if(!string.IsNullOrEmpty(Id))
            {
                userId = Id;
            }
            else 
            {
                userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            var user = _db.Users.Where(u=>u.Id == userId)
            .Select(u => new ChangePasswordVM
            {
                
            })
            .FirstOrDefault();

            if(user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM Input, string? Id = "")
        {
            string userId_nat = _userManager.GetUserId(User);
            var loggedUser = _db.Users.Where(u=>u.Id == userId_nat).FirstOrDefault();
            
            if (ModelState.IsValid)
            {
                    string userId =  "";
                    if(!string.IsNullOrEmpty(Id))
                    {
                        userId = Id;
                    }
                    else 
                    {
                        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    var userData = _db.Users.Find(userId);


                var result = await _userManager.ChangePasswordAsync(userData, Input.OldPassword, Input.Password);
                
                if(result.Errors.Count() > 0)
                {
                    ModelState.AddModelError("OldPassword", "Old Password is incorrect");
                    return View(Input);
                }
                
                if(!string.IsNullOrEmpty(Id)) 
                {
                    if(userData.NationalId == "11")
                    {
                        return RedirectToAction("Index", "Users");
                    }
                
                }
                
                return RedirectToAction("Settings");

                }   

                return View(Input);
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
                var tlic = _db.TechearMemberShips
                .Include("TechearMemberShipTests")
                .Where(m=>m.AppUser.Id == userId).FirstOrDefault();
                userSetting.TeacherLic.ExpireDate = tlic.ExpireDate.HasValue ?   tlic.ExpireDate.Value.ToShortDateString() : "";
                userSetting.TeacherLic.FinalApprove = tlic.FinalApprove;
                userSetting.TeacherLic.Status = tlic.Status;
                userSetting.TeacherLic.Serial = tlic.SerialNumber;
                
                var vmTestsList = new List<TechearMemberShipTestVM>();
                if(tlic != null)
                {
                    vmTestsList = tlic.TechearMemberShipTests.Select(v => new TechearMemberShipTestVM {
                    ExpireDate = v.ExpireDate.HasValue ?  v.ExpireDate.Value.ToShortDateString() : "",
                    FinalApprove = v.FinalApprove,
                    Status = v.Status,
                    Serial = v.SerialNumber,
                    TeachingType_String = GlobalHelpers.getTeachingType(v.TeachingType)

                }).ToList();
                }
                
                
                

                if(tlic != null)userSetting.TeacherLic.CreationDate = tlic.CertaficateDate.HasValue ? tlic.CertaficateDate.Value.ToShortDateString(): "";
                if(tlic != null)userSetting.TeacherLic.TechearMemberShipTests = vmTestsList;


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
                userSetting.MemshipCard.IssueDate = memC.ExpireDate.HasValue ?   memC.ExpireDate.Value.AddYears(-1).ToShortDateString() : "";
            }                

            return View(userSetting);
        }

        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DownloadUserData(string x)
        {
            var htmlContent = "";
            //1var Rendered = new ChromePdfRenderer(); 
            //2using var PDF = Rendered.RenderHtmlAsPdf(htmlContent);
            string fileName = "techer_licnese{tech.SerialNumber}.pdf";
            var attachmentFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", fileName);
            //3var attachmentFile = $"G:/dev/ramz/yoga/sourcecode/yogamvccode/yoga/wwwroot/assets/{fileName}";

            //4PDF.SaveAs(attachmentFile);

            return View();                        
        }

        public IActionResult Users()
        {
            var result = _db.Users.Include("Country")
            .Where(u=>u.NationalId != "11")
            .ToList();
            return View(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                // If the user is found AND Email is confirmed
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Generate the reset password token
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    // Log the password reset link
                    //logger.Log(LogLevel.Warning, passwordResetLink);

                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);

                    string content = $"To Reset Your Password Please click here <a href='{passwordResetLink}'>Clicking Here</a>";
                    var emailMessage = new EmailMessage
                    {
                        ToEmailAddresses = new List<string> { user.Email },
                        Subject = "SAUDI YOGA COMMITTEE - Reset Password",
                        Body = content
                    };

                    _emailSender.SendEmailBySendGrid(emailMessage);


                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string email)  
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)  
        {

            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        } 

        public IActionResult ExportToExcel(int Id)
        {
            
            var result = _db.Users
            
            .Select(t => new
            {
                UserId = t.Id,
                FirstName = t.FirstName,
                MiddleName = t.MiddleName,
                LastName = t.LastName,
                Phone = t.PhoneNumber,
                Email = t.Email,
                Nationality = t.Country.EnName,
                City = t.City.EnName
            })
            .ToList();
            var stream = new MemoryStream();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromCollection(result, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Users data {DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}