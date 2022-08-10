using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yoga.Data;
using yoga.Models;
using yoga.ViewModels;

namespace yoga.Controllers
{
    public class TeacherLicController: Controller
    {
        private readonly YogaAppDbContext _db;
        private readonly ILogger<TeacherLicController> _logger;
        private readonly UserManager<AppUser> _userManager;


        public TeacherLicController(ILogger<TeacherLicController> logger, YogaAppDbContext db, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            IEnumerable<TechearMemberShip> lics = _db.TechearMemberShips;
            return View(lics);
        }
        
        [Authorize]
        public IActionResult Create()
        {
            var member = IfTeacherExists();
            if(member != null) return View(member);
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(TechearMemberShipVM obj, IFormFile CertficateFiles)
        {
            ModelState.Remove("ReceiptCopy");
            ModelState.Remove("CertficateFiles");
            ModelState.Remove("Agreement");
            

            if(ModelState.IsValid)
            {
                if(!obj.Agreement)
                {
                    ModelState.AddModelError("Agreement", " Terms Conditions and Acknowledged");
                    return View(obj);
                }
                // Upload images
                string fileName_rec = "";
                string fileName_cert = "";
                

                if(CertficateFiles != null && CertficateFiles.Length > 0)
                {
                    fileName_cert = Path.GetFileName(CertficateFiles.FileName);
                    var filePath_cert = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_cert);
                    using (var fileSrteam = new FileStream(filePath_cert, FileMode.Create))
                    {
                        await CertficateFiles.CopyToAsync(fileSrteam);
                    }
                }
                
                // get logened user
                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
                if(loggedUser == null)
                {
                    ModelState.AddModelError("", "User Not Exist");
                    return View();
                }

                TechearMemberShip entity = new TechearMemberShip();
                entity.AppUser = loggedUser;
                entity.Id = userId;
                entity.EducationLevel = obj.EducationLevel;
                entity.SocialMediaAccounts = obj.SocialMediaAccounts;
                entity.PersonalWebSite = obj.PersonalWebSite;
                entity.TeachingType = obj.TeachingType;
                entity.ExpYears = obj.ExpYears;
                entity.AccreditedHours = obj.AccreditedHours;
                entity.SchoolLocation = obj.SchoolLocation;
                entity.CertaficateDate = obj.CertaficateDate;
                entity.SchoolName = obj.SchoolName;
                entity.SchoolLink = obj.SchoolLink;
                entity.SchoolSocialMediaAccount = obj.SchoolSocialMediaAccount;
                entity.CertficateFiles = string.IsNullOrEmpty(CertficateFiles?.FileName) ? "" : CertficateFiles.FileName;
                entity.Name =obj.Name;

                _db.TechearMemberShips.Add(entity);
                _db.SaveChanges();

                // Add user to Teacher role
                await _userManager.AddToRoleAsync(loggedUser, "Teacher");
                ViewData["Saved"] = "Thanks, Your request sent successfully, we will review your it and approve your Membership ASAP.";
                return View(obj);
            }
            return View();
        }

        private TechearMemberShip? IfTeacherExists()
        {
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId)) return null;

            var member = _db.TechearMemberShips.Where(t => t.AppUser.Id == userId).FirstOrDefault();
            if(member != null) return member;
            return null;
        }

        public IActionResult Detail(int? id)
        {
            if(id == null || id == 0) return NotFound();
            var obj = _db.TechearMemberShips.Find(id);
            obj.EducationLevel_String = getEducationLevel((int)obj.EducationLevel);
            obj.TeachingType_string = getTeachingType(obj.TeachingType);
            if(obj == null) return NotFound();

            return View(obj);
        }

        string getEducationLevel(int levelId)
        {
            if(levelId == 1)
            {
                return "Level1";
            }
            else if(levelId == 2)
            {
                return "Level2";
            }
            else if(levelId == 3)
            {
                return "Level3";
            }
            else {
                return "";
            }
        }
        string getTeachingType(int typeId)
        {
            if(typeId == 1)
            {
                return "Yin";
            }
            else if(typeId == 2)
            {
                return "Prenatal";
            }
            else if(typeId == 3)
            {
                return "Therapy";
            }
            else if(typeId == 4)
            {
                return "Aerial";
            }
            else if(typeId == 5)
            {
                return "Hatha";
            }
            else if(typeId == 6)
            {
                return "Ashtanga";
            }
            else if(typeId == 7)
            {
                return "Vinyasa Flow";
            }
            else if(typeId == 8)
            {
                return "Iyengar";
            }
            else {
                return "";
            }
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detail(string Approve, string reason, int Info, int PayExamFees, int PayLicFees, int TakeExam,
        int PassExam, int MemId)
        {
            ModelState.Remove("Approve");
            ModelState.Remove("reason");
            ModelState.Remove("PayLicFees");

            if(ModelState.IsValid)
            {
                string content = "";
                string subject = "SAUDI YOGA COMMITTEE";
                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
                var tech = _db.TechearMemberShips.Include("AppUser").Where(m=>m.MemId == MemId).FirstOrDefault();

                if(!string.IsNullOrEmpty(Approve))
                {
                    if(Info == 1)
                    {
                        tech.Status = (int)StatusEnum.Approved;
                        content = "Congratuilation, You information is approved, next step is take the exam at the below address.. ";
                    }
                     
                    if(PayExamFees == 1)
                    {
                        tech.PayExamFees = true;
                        content  += "Your Exam Fees Approved, next step is take the exam at the below address.. ";
                    }

                    if(PayLicFees == 1)
                    {
                        tech.PayFees = true;
                        tech.FinalApprove = true;
                        content += "Congratuilation, Your Are now Licensed SAUDI YOGA COMMITTEE Teacher. ";
                    }
                    if(TakeExam == 1) 
                    {
                        content += "Thank you for taking the SAUDI YOGA COMMITTEE Teacher Licese exam. ";
                        tech.TakeExam = true;
                    }
                    if(PassExam == 1)
                    {
                        content = "Congratuilation, Your Are Passed The SAUDI YOGA COMMITTEE Teacher license exam. ";
                        tech.PassExam = true;
                    }
                    


                }
                else {
                    tech.Status = (int)StatusEnum.Rejected;
                    content = $"Sorry your data rejected for the below reason <p>{reason}</p>";
                }

                

                _db.TechearMemberShips.Update(tech);
                int rowAffect = _db.SaveChanges();
                // Send email
                var memUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {tech.AppUser.Email},
                    Subject = subject,
                    Body = content
                };
                
                try
                {
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    if(rowAffect == 1)
                    _emailSender.SendEmailAsync(emailMessage);
                    return RedirectToAction("Detail", "TeacherLic", new {id=MemId});
                }
                catch (System.Exception ex)
                {
                    
                    ModelState.AddModelError("", ex.Message);
                }
                return View();
            }
            return View();
        }


        public IActionResult PayExamFees()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayExamFees(IFormFile Receit)
        {
            if(ModelState.IsValid)
            {
                string fileName_rec = "";
                if(Receit != null && Receit.Length > 0)
                {
                    fileName_rec = Path.GetFileName(Receit.FileName);
                    var filePath_rec = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_rec);
                    using (var fileSrteam = new FileStream(filePath_rec, FileMode.Create))
                    {
                        await Receit.CopyToAsync(fileSrteam);
                    }
                }

                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
                var tech = _db.TechearMemberShips.Where(t=>t.Id == userId).FirstOrDefault();
                //tech.PayExamFees = true;
                tech.ReceiptCopy  = fileName_rec;
                _db.TechearMemberShips.Update(tech);
                _db.SaveChanges();
                ViewData["Saved"] = "Thank you, We going to review your informatino ASAP.";
                return View();
            }
            return View();
        }

        public IActionResult PayLicFees()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayLicFees(IFormFile Receit)
        {
            if(ModelState.IsValid)
            {
                string fileName_rec = "";
                if(Receit != null && Receit.Length > 0)
                {
                    fileName_rec = Path.GetFileName(Receit.FileName);
                    var filePath_rec = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_rec);
                    using (var fileSrteam = new FileStream(filePath_rec, FileMode.Create))
                    {
                        await Receit.CopyToAsync(fileSrteam);
                    }
                }

                var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
                var tech = _db.TechearMemberShips.Where(t=>t.Id == userId).FirstOrDefault();
                //tech.PayExamFees = true;
                tech.ReceiptCopyLic  = fileName_rec;
                _db.TechearMemberShips.Update(tech);
                _db.SaveChanges();
                ViewData["Saved"] = "Thank you, We going to review your informatino ASAP.";
                return View();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}