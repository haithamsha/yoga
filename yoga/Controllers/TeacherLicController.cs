using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
            IEnumerable<TechearMemberShip> lics = _db.TechearMemberShips.Include("AppUser");
            return View(lics);
        }
        
        private List<SelectListItem> getEducationLevels()
        {
            List<SelectListItem> levels =new List<SelectListItem>();
            levels.Add(new SelectListItem{
                Text = "Select Level",
                Value = "0"
            });

            levels.Add(new SelectListItem{
            Text = "Level 1",
            Value = "1"
            });

            levels.Add(new SelectListItem{
            Text = "Level 2",
            Value = "2"
            });

            levels.Add(new SelectListItem{
            Text = "Level 3",
            Value = "3"
            });
            return levels;
        }

        private List<SelectListItem> getTeachingTypes()
        {
            List<SelectListItem> t_Types =new List<SelectListItem>();
            t_Types.Add(new SelectListItem{
                Text = "Select Teaching Type",
                Value = "0"
            });

            t_Types.Add(new SelectListItem{
            Text = "Yin",
            Value = "1"
            });

            t_Types.Add(new SelectListItem{
            Text = "Prenatal",
            Value = "2"
            });

            t_Types.Add(new SelectListItem{
            Text = "Therapy",
            Value = "3"
            });
            t_Types.Add(new SelectListItem{
            Text = "Aerial",
            Value = "4"
            });
            t_Types.Add(new SelectListItem{
            Text = "Hatha",
            Value = "5"
            });
            t_Types.Add(new SelectListItem{
            Text = "Ashtanga",
            Value = "6"
            });
            t_Types.Add(new SelectListItem{
            Text = "Vinyasa Flow",
            Value = "7"
            });
            t_Types.Add(new SelectListItem{
            Text = "Iyengar",
            Value = "8"
            });
            return t_Types;
        }

        [Authorize]
        public IActionResult Create()
        {
            var member = IfTeacherExists();
            if(member != null) return View(member);
            var vm = new TechearMemberShipVM();
            vm.EducationLevels = getEducationLevels();
            vm.TeachingTypes = getTeachingTypes();
            return View(vm);
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
            ModelState.Remove("ExamDetails");
            ModelState.Remove("SchoolSocialMediaAccount");
            ModelState.Remove("Name");
            ModelState.Remove("PersonalWebSite");
            ModelState.Remove("EducationLevels"); 
            ModelState.Remove("TeachingTypes");
            obj.Name = "tst";

            if(obj.TeachingType == 0)
            {
                obj.EducationLevels = getEducationLevels();
                obj.TeachingTypes = getTeachingTypes();
                ModelState.AddModelError("TeachingType", " Teaching Type is required");
            }
            if(obj.EducationLevel == 0)
            {
                obj.EducationLevels = getEducationLevels();
                obj.TeachingTypes = getTeachingTypes();
                ModelState.AddModelError("EducationLevel", " Education Level is required");
            }

            if(ModelState.IsValid)
            {
                if(!obj.Agreement)
                {
                    obj.EducationLevels = getEducationLevels();
                    obj.TeachingTypes = getTeachingTypes();
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
                    return View(obj);
                }

                TechearMemberShip entity = new TechearMemberShip();
                entity.AppUser = loggedUser;
                entity.Id = userId;
                entity.EducationLevel = obj.EducationLevel;
                entity.SocialMediaAccounts = obj.SocialMediaAccounts;
                entity.PersonalWebSite = obj.PersonalWebSite;
                entity.TeachingType = obj.TeachingType;
                entity.ExpYears = obj.ExpYears.Value;
                entity.AccreditedHours = obj.AccreditedHours.Value;
                entity.SchoolLocation = obj.SchoolLocation;
                entity.CertaficateDate = obj.CertaficateDate.Value;
                entity.SchoolName = obj.SchoolName;
                entity.SchoolLink = obj.SchoolLink;
                entity.SchoolSocialMediaAccount = obj.SchoolSocialMediaAccount;
                entity.CertficateFiles = string.IsNullOrEmpty(CertficateFiles?.FileName) ? "" : CertficateFiles.FileName;
                entity.Name =obj.Name;

                _db.TechearMemberShips.Add(entity);
                _db.SaveChanges();

                // Add user to Teacher role
                await _userManager.AddToRoleAsync(loggedUser, "Teacher");
                ViewData["Saved"] = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";
                return RedirectToAction("DataSaved");
            }
            obj.EducationLevels = getEducationLevels();
            obj.TeachingTypes = getTeachingTypes();
            return View(obj);
        }

        public IActionResult DataSaved()
        {
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
            var obj = _db.TechearMemberShips.Include("AppUser").Where(t=>t.MemId == id).SingleOrDefault();
            obj.EducationLevel_String = getEducationLevel((int)obj.EducationLevel);
            obj.TeachingType_string = getTeachingType(obj.TeachingType);
            if(obj == null) return NotFound();

            return View(obj);
        }

        static string getEducationLevel(int levelId)
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
        static string getTeachingType(int typeId)
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
        int PassExam, int MemId, decimal LicFeesPrice, string ExamLocation)
        {
            ModelState.Remove("Approve");
            ModelState.Remove("reason");
            ModelState.Remove("PayLicFees");
            ModelState.Remove("LicFeesPrice");
            ModelState.Remove("ExamLocation");

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
                        tech.ExamLocation = ExamLocation;
                        content = $"Congratuilation, You information is approved, next step is take the exam please go to your profile and pay the exam fees. </p>";
                    }
                    if(PayExamFees == 1)
                    {
                        if(string.IsNullOrEmpty(ExamLocation))
                        {
                            ModelState.AddModelError("", "Please Insert The Exam Location and Date!");
                            return View(tech);
                        }
                        tech.PayExamFees = true;
                        content  += "Your Exam Fees Approved, next step is take the exam at the below address.. ";
                    }

                    if(PayLicFees == 1)
                    {
                        tech.PayFees = true;
                        tech.FinalApprove = true;
                        tech.ExpireDate = DateTime.Now.AddYears(1);
                         // Generate card serial
                        var serials = _db.TechearMemberShips.Select(m=>m.SerialNumber).ToList();
                        string serialNumber = YogaUtilities.GenerateSerialNumber(serials);
                        tech.SerialNumber = serialNumber;

                        content += @$"<div>
                        <p>
                        Congratuilation, Your Are now Licensed SAUDI YOGA COMMITTEE Teacher.
                        </p>
                        </div>
                        <div style='text-align: center; width:200px;height: 270px; padding:30px;
    background-color: #efece5;color:#b77b57;font-family: 'Courier New', Courier, monospace;'>
        <div style='padding-bottom: 20px;'>
            <img width='80px' src='https://iili.io/r1zcZb.png'
            alt='Yoga'> 
        </div>
        <div>
            <img swidth='80px' src='https://iili.io/r1uyHN.png' alt='Yoga'>
        </div>
       <div >
        <div>
            {tech.AppUser.FirstName} {tech.AppUser.LastName}
        </div>
        <div>
            ID: {serialNumber}
        </div>
        <div>
            Validity: {DateTime.Now.AddYears(1).ToShortDateString()}
</div></div></div>
                        ";
                    }
                    if(TakeExam == 1) 
                    {
                        content += "Thank you for taking the SAUDI YOGA COMMITTEE Teacher Licese exam. ";
                        tech.TakeExam = true;
                    }
                    if(PassExam == 1)
                    {
                        if(LicFeesPrice <= 0)
                        {
                            ModelState.AddModelError("", "Please Insert The License Fees!");
                            return View(tech);
                        }
                        content = "Congratuilation, Your Are Passed The SAUDI YOGA COMMITTEE Teacher license exam. ";
                        tech.PassExam = true;
                        tech.LicenseFeesPrice = LicFeesPrice;
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
                    _emailSender.SendEmailBySendGrid(emailMessage);
                    return RedirectToAction("Detail", "TeacherLic", new {id=MemId});
                }
                catch (System.Exception ex)
                {
                    
                    ModelState.AddModelError("Error, Please Try Again ", ex.Message);
                    return View(tech);
                }
                
            }
            return View();
        }


        public IActionResult PayExamFees()
        {
            TechearMemberShipVM vm = new TechearMemberShipVM();
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tech = _db.TechearMemberShips.Include("AppUser")
            .Where(m=>m.AppUser.Id == userId).FirstOrDefault();

            if(tech == null){
                return NotFound();
            }

            vm.ExamDetails = tech != null ? tech.ExamLocation : "";

            return View(vm);
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
                return RedirectToAction("Confirmation");
            }
            return View();
        }

        public ActionResult ConfirmPayExamFees()
        {
            return View();
        }

        public IActionResult PayLicFees()
        {
            TechearMemberShipVM vm = new TechearMemberShipVM();
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tech = _db.TechearMemberShips.Where(t=>t.Id == userId).FirstOrDefault();
            if(tech == null ) return NotFound();
            vm.LicenseFeesPrice = tech.LicenseFeesPrice;
            return View(vm);
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
                return RedirectToAction("Confirmation");
                return View();
            }
            return View();
        }

        public IActionResult ExportToExcel()
        {
            var result = _db.TechearMemberShips
            .Include("AppUser")
            .Select( t => new {
                Name = $"{t.AppUser.FirstName} {t.AppUser.MiddleName} {t.AppUser.LastName}",
                ExperienceYears = t.ExpYears,
                AccreditedHours= t.AccreditedHours,
                EducationLevel = getEducationLevel((int)t.EducationLevel),
                TeachingType = getTeachingType((int)t.TeachingType),
                PayExamFees = t.PayExamFees == true ? "Yes": "No",
                PayLicFees = t.PayFees  == true ? "Yes": "No",
                FinalApprove = t.Status == 1  ? "Pending" : "Approved",
                CurrentInformationStatus = getCurrentStatus(t.Status),
                SerialNumber = string.IsNullOrEmpty(t.SerialNumber) ? "Not Generated Yet" : t.SerialNumber,
                ExpireDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.ToShortDateString() : ""
            })
            .ToList();
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromCollection(result, true);
                package.Save();
            }
            stream.Position= 0;
            string excelName = $"Techers Licenses data {DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        static string getCurrentStatus(int statusId)
        {
            if(statusId == 1) return "Pending";
            if(statusId == 2) return "Approved";
            if(statusId == 3) return "Rejected";
            return "Pending";
        }
    }
}