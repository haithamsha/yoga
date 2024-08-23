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
using System.Linq;
using yoga.Helpers;
using System.Globalization;

namespace yoga.Controllers
{
    public class TeacherLicController : Controller
    {
        private readonly YogaAppDbContext _db;
        private readonly ILogger<TeacherLicController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly NotificationHelper _notificationHelper;
        private readonly IWFHistoryManager _wfHistoryManager;

        public TeacherLicController(ILogger<TeacherLicController> logger,
        YogaAppDbContext db, UserManager<AppUser> userManager, 
        IWFHistoryManager wfHistoryManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _notificationHelper = new NotificationHelper(_db);
            _wfHistoryManager = wfHistoryManager;
        }

        public IActionResult CreateDemo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDemo(CreateDemoVM obj)
        {
            return View(obj);
        }

        public IActionResult PreIndex()
        {
            IEnumerable<TechearMemberShip> lics = _db.TechearMemberShips
            .Include("TechearMemberShipTests")
            .Include(t=>t.AppUser);
            return View(lics);
        }
        public IActionResult Index(int Id)
        {
            IEnumerable<TechearMemberShipTest> lics = _db.techearMemberShipTests
            .Include("TechearMemberShip")
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(m=>m.TechearMemberShip.MemId == Id)
            .ToList();

            foreach (var item in lics)
            {
                item.TeachingType_string = GlobalHelpers.getTeachingType(item.TeachingType);
                //item.MemId = 
            }
            return View(lics);
        }

        private List<SelectListItem> getEducationLevels()
        {
            var culture = Request.HttpContext.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();

            List<SelectListItem> levels = new List<SelectListItem>();
            levels.Add(new SelectListItem
            {
                Text = culture.RequestCulture.UICulture.Name == "en" ? "Select Level" : "اختر المستوي",
                Value = string.Empty
            });

            levels.Add(new SelectListItem
            {
                Text = "Level 1 - 100 hrs",
                Value = "1"
            });

            levels.Add(new SelectListItem
            {
                Text = "Level 2 - 200 hrs",
                Value = "2"
            });

            levels.Add(new SelectListItem
            {
                Text = "Level 3 - 300 hrs",
                Value = "3"
            });
            return levels;
        }

        public List<SelectListItem> getTeachingTypes()
        {
            List<SelectListItem> t_Types = new List<SelectListItem>();
            // t_Types.Add(new SelectListItem
            // {
            //     Text = "Select Teaching Type",
            //     Value = string.Empty
            // });

            t_Types.Add(new SelectListItem
            {
                Text = "Yin",
                Value = "1"
            });

            t_Types.Add(new SelectListItem
            {
                Text = "Prenatal",
                Value = "2"
            });

            t_Types.Add(new SelectListItem
            {
                Text = "Therapy",
                Value = "3"
            });
            t_Types.Add(new SelectListItem
            {
                Text = "Aerial",
                Value = "4"
            });
            t_Types.Add(new SelectListItem
            {
                Text = "Hatha",
                Value = "5"
            });
            t_Types.Add(new SelectListItem
            {
                Text = "Ashtanga",
                Value = "6"
            });
            t_Types.Add(new SelectListItem
            {
                Text = "Vinyasa Flow",
                Value = "7"
            });
            t_Types.Add(new SelectListItem
            {
                Text = "Iyengar",
                Value = "8"
            });
            return t_Types;
        }

        [Authorize]
        public IActionResult Create()
        {
            var member = IfTeacherExists(0);
            if (member != null) return View(member);
            var vm = new TechearMemberShipVM();
            vm.EducationLevels = getEducationLevels();
            vm.TeachingTypes = getTeachingTypes();
            ViewBag.TeachingTypes = getTeachingTypes();
            
            // if (ViewData["SavedFiles"] != null)
            // {
            //     vm.CertficateFiles = ViewData["SavedFiles"].ToString();
            // }
            vm.TechearMemberShipTestVMs.Add(new TechearMemberShipTestVM{TestId=1,ExamDetails = "", AccreditedHours = 0});
            return View(vm);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(TechearMemberShipVM obj, IFormFile CertficateFiles,
        IFormFile Image, string[] TeachingTypesList)
        {
            ModelState.Remove("TeachingTypes");
            ModelState.Remove("ReceiptCopy");
            ModelState.Remove("CertficateFiles");
            ModelState.Remove("Agreement");
            ModelState.Remove("ExamDetails");
            ModelState.Remove("SchoolSocialMediaAccount");
            ModelState.Remove("Name");
            ModelState.Remove("PersonalWebSite");
            ModelState.Remove("EducationLevels");
            ModelState.Remove("Image");
            ModelState.Remove("TeachingType");
            obj.Name = "tst";
            ModelState.Remove("TeachingTypesList");
            foreach (var item in obj.TechearMemberShipTestVMs)
            {
                ModelState.Remove("ExamDetails");
                ModelState.Remove("TeachingTypes");
                ModelState.Remove("TeachingTypesList");
            }

            // if (TeachingTypesList.Count() == 0)
            // {
            //     obj.EducationLevels = getEducationLevels();
            //     obj.TeachingTypes = getTeachingTypes();
            //     ModelState.AddModelError("TeachingType", " Teaching Type is required");
            // }
            if (obj.EducationLevel == 0)
            {
                obj.EducationLevels = getEducationLevels();
                obj.TeachingTypes = getTeachingTypes();
                ModelState.AddModelError("EducationLevel", " Education Level is required");
            }

            if (ModelState.IsValid)
            {
                if (!obj.Agreement)
                {
                    obj.EducationLevels = getEducationLevels();
                    obj.TeachingTypes = getTeachingTypes();
                    ModelState.AddModelError("Agreement", " Terms Conditions and Acknowledged");
                    return View(obj);
                }

                


                // get logened user
                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (loggedUser == null)
                {
                    ModelState.AddModelError("", "User Not Exist");
                    return View(obj);
                }

                // validate user image
                if (string.IsNullOrEmpty(loggedUser.UserImage))
                {
                    // Upload image
                    string fileName = "";
                    if (Image != null && Image.Length > 0)
                    {
                        fileName = Path.GetFileName(Image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName);
                        using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileSrteam);
                        }
                        // update user image column
                        var user = _db.Users.Find(loggedUser.Id);
                        if (user != null)
                        {
                            user.UserImage = fileName;
                            _db.Users.Update(user);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Image is rquired");
                        obj.EducationLevels = getEducationLevels();
                        obj.TeachingTypes = getTeachingTypes();
                        return View(obj);
                    }
                }

                TechearMemberShip entity = new TechearMemberShip();
                entity.AppUser = loggedUser;
                entity.Id = userId;
                entity.EducationLevel = (EducationLevelEnum)obj.EducationLevel;
                entity.SocialMediaAccounts = obj.SocialMediaAccounts;
                entity.PersonalWebSite = obj.PersonalWebSite;
                if(entity.TeachingType != null) entity.TeachingType = obj.TeachingType.Value;
                entity.ExpYears = obj.ExpYears.Value;
                
                //entity.CertficateFiles = uploadFiles == null ? "" : string.Join(",", uploadFiles);
                entity.Name = obj.Name;

                // validate innter cert items
                if(obj.TechearMemberShipTestVMs.Count != Request.Form.Files.Count)
                {
                    ModelState.AddModelError("", "Certification file is required for each certaficate");
                    obj.EducationLevels = getEducationLevels();
                    obj.TeachingTypes = getTeachingTypes();
                    return View(obj);
                }

                foreach (var item in obj.TechearMemberShipTestVMs)
                {
                    // Upload images
                    string fileName_cert = "";

                    List<string> uploadFiles = new List<string>();

                    if (CertficateFiles != null)
                    {
                        var filePath_cert = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/assets", "images");
                        IFormFile fileItem = CertficateFiles;

                        fileName_cert = Path.GetFileName(fileItem.FileName);
                        using (var fileSrteam = new FileStream(Path.Combine(filePath_cert, fileName_cert), FileMode.Create))
                        {
                            // await CertficateFiles.CopyToAsync(fileSrteam);
                            fileItem.CopyTo(fileSrteam);
                            uploadFiles.Add(fileName_cert);
                        }

                    }

                    entity.TechearMemberShipTests.Add(new TechearMemberShipTest{TeachingType = item.TeachingType.Value,
                    AccreditedHours = item.AccreditedHours, CertaficateDate = item.CertaficateDate, CertficateFiles = fileName_cert,
                    SchoolLocation =  item.SchoolLocation, SchoolName = item.SchoolName, SchoolLink = item.SchoolLink, 
                    SchoolSocialMediaAccount = item.SchoolSocialMediaAccount});

                
                }
                
                _db.TechearMemberShips.Add(entity); 
                int rowAffect = _db.SaveChanges();

                // Add user to Teacher role
                await _userManager.AddToRoleAsync(loggedUser, "Teacher");
                ViewData["Saved"] = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";

                // Add notification
                string body = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";
                _notificationHelper.AddNotify(new Notification
                {
                    AppUser = loggedUser,
                    Body = body,
                    Title = "Create Teacher Licence",
                    CreationDate = DateTime.Now,
                    IsRead = false
                });

                // add wfhistory
                WFHistory wfHistory = new WFHistory();
                wfHistory.AppUser = loggedUser;
                wfHistory.WFHistoryType = WFHistoryTypeEnum.CreatTeacherLicense;
                wfHistory.RecordId = rowAffect;
                wfHistory.CreationDate = DateTime.Now;
                wfHistory.ModuleName = "TeacherLic";
                wfHistory.Description = "Create TeacherLicense";
                int wfSaved = _wfHistoryManager.Save(wfHistory);

                // Send Email
                try
                {
                    var emailMessage = new EmailMessage
                    {
                        ToEmailAddresses = new List<string> { loggedUser.Email },
                        Subject = "SAUDI YOGA COMMITTEE",
                        Body = body
                    };
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    emailMessage.Body = body;
                    if (rowAffect == 1)
                        _emailSender.SendEmailBySendGrid(emailMessage);
                }
                catch (System.Exception ex)
                {

                    throw;
                }

                return RedirectToAction("DataSaved");
            }
            obj.EducationLevels = getEducationLevels();
            obj.TeachingTypes = getTeachingTypes();
            return View(obj);
        }


        [HttpPost]
        public JsonResult createAjax(TestVM testVM)
        {
            return Json(testVM);
        }

        public IActionResult CreateSingleLic()
        {
            var member = IfTeacherExists(0);
            if (member != null) return View(member);
            var vm = new TechearMemberShipTestVM();
            vm.TeachingTypes = getTeachingTypes();
            ViewBag.TeachingTypes = getTeachingTypes();
            
            vm.TestId=1; vm.ExamDetails = ""; vm.AccreditedHours = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateSingleLic(TechearMemberShipTestVM obj, IFormFile CertficateFiles,
        IFormFile Image, string[] TeachingTypesList, int memId)
        {
            ModelState.Remove("ReceiptCopy");
            ModelState.Remove("CertficateFiles");
            ModelState.Remove("ExamDetails");
            ModelState.Remove("SchoolSocialMediaAccount");
            ModelState.Remove("Name");
            ModelState.Remove("TeachingTypes");
            ModelState.Remove("Image");
            ModelState.Remove("TeachingType");
            ModelState.Remove("TeachingTypesList");
            ModelState.Remove("ExamDetails");
            ModelState.Remove("TeachingTypes");
            ModelState.Remove("TeachingTypesList");

            if (ModelState.IsValid)
            {
                // get logened user
                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (loggedUser == null)
                {
                    ModelState.AddModelError("", "User Not Exist");
                    return View(obj);
                }

                if (obj.TeachingType == 0)
                {
                    obj.TeachingTypes = getTeachingTypes();
                    ModelState.AddModelError("TeachingType", " Teaching Type is required");
                }

                // validate user image
                if (string.IsNullOrEmpty(loggedUser.UserImage))
                {
                    // Upload image
                    string fileName = "";
                    if (Image != null && Image.Length > 0)
                    {
                        fileName = Path.GetFileName(Image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName);
                        using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileSrteam);
                        }
                        // update user image column
                        var user = _db.Users.Find(loggedUser.Id);
                        if (user != null)
                        {
                            user.UserImage = fileName;
                            _db.Users.Update(user);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Image is rquired");
                        return View(obj);
                    }
                }

                var entity = _db.TechearMemberShips
                .Where(m=>m.MemId == memId)
                .Include(m=> m.AppUser)
                .SingleOrDefault();

                // validate innter cert items
                if(Request.Form.Files.Count == 0)
                {
                    ModelState.AddModelError("", "Certification file is required for each certaficate");
                    return View(obj);
                }

                // Upload images
                string fileName_cert = "";

                List<string> uploadFiles = new List<string>();

                if (CertficateFiles != null)
                {
                    var filePath_cert = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/assets", "images");
                    IFormFile fileItem = CertficateFiles;

                    fileName_cert = Path.GetFileName(fileItem.FileName);
                    using (var fileSrteam = new FileStream(Path.Combine(filePath_cert, fileName_cert), FileMode.Create))
                    {
                        // await CertficateFiles.CopyToAsync(fileSrteam);
                        fileItem.CopyTo(fileSrteam);
                        uploadFiles.Add(fileName_cert);
                    }

                }

                entity.TechearMemberShipTests.Add(new TechearMemberShipTest{TeachingType = obj.TeachingType.Value,
                AccreditedHours = obj.AccreditedHours, CertaficateDate = obj.CertaficateDate, CertficateFiles = fileName_cert,
                SchoolLocation =  obj.SchoolLocation, SchoolName = obj.SchoolName, SchoolLink = obj.SchoolLink, 
                SchoolSocialMediaAccount = obj.SchoolSocialMediaAccount});
                
                _db.TechearMemberShips.Update(entity); 
                int rowAffect = _db.SaveChanges();

                // Add user to Teacher role
                await _userManager.AddToRoleAsync(loggedUser, "Teacher");
                ViewData["Saved"] = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";

                // Add notification
                string body = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";
                _notificationHelper.AddNotify(new Notification
                {
                    AppUser = loggedUser,
                    Body = body,
                    Title = "Create Teacher Licence",
                    CreationDate = DateTime.Now,
                    IsRead = false
                });

                // add wfhistory
                WFHistory wfHistory = new WFHistory();
                wfHistory.AppUser = loggedUser;
                wfHistory.WFHistoryType = WFHistoryTypeEnum.CreatTeacherLicense;
                wfHistory.RecordId = rowAffect;
                wfHistory.CreationDate = DateTime.Now;
                wfHistory.ModuleName = "TeacherLic";
                wfHistory.Description = "Add New Certificate to  TeacherLicense";
                int wfSaved = _wfHistoryManager.Save(wfHistory);

                // Send Email
                try
                {
                    var emailMessage = new EmailMessage
                    {
                        ToEmailAddresses = new List<string> { loggedUser.Email },
                        Subject = "SAUDI YOGA COMMITTEE",
                        Body = body
                    };
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    emailMessage.Body = body;
                    if (rowAffect == 1)
                        _emailSender.SendEmailBySendGrid(emailMessage);
                }
                catch (System.Exception ex)
                {

                    throw;
                }

                return RedirectToAction("DataSaved");
            }
            obj.TeachingTypes = getTeachingTypes();
            return View(obj);
        }

        public IActionResult DataSaved()
        {
            return View();
        }

        private TechearMemberShipTest? IfTeacherExists(int testId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return null;

            var member = _db.techearMemberShipTests
                .Include(t=>t.TechearMemberShip)
                .Where(t => t.TechearMemberShip.Id == userId && t.TestId == testId)
                .FirstOrDefault();
            if (member != null) return member;
            return null;

            
        }

        static string getEducationLevel(int levelId)
        {
            if (levelId == 1)
            {
                return "Level 1 - 100 hrs";
            }
            else if (levelId == 2)
            {
                return "Level 2 - 200 hrs";
            }
            else if (levelId == 3)
            {
                return "Level 3 - 300 hrs";
            }
            else
            {
                return "";
            }
        }
        static string getTeachingType(int typeId)
        {
            if (typeId == 1)
            {
                return "Yin";
            }
            else if (typeId == 2)
            {
                return "Prenatal";
            }
            else if (typeId == 3)
            {
                return "Therapy";
            }
            else if (typeId == 4)
            {
                return "Aerial";
            }
            else if (typeId == 5)
            {
                return "Hatha";
            }
            else if (typeId == 6)
            {
                return "Ashtanga";
            }
            else if (typeId == 7)
            {
                return "Vinyasa Flow";
            }
            else if (typeId == 8)
            {
                return "Iyengar";
            }
            else
            {
                return "";
            }
        }

        public IActionResult Detail(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var obj = _db.techearMemberShipTests
            .Include(t=>t.TechearMemberShip)
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(t => t.TestId == id)
            .SingleOrDefault();

            obj.TechearMemberShip.EducationLevel_String = getEducationLevel((int)obj.TechearMemberShip.EducationLevel);
            obj.TeachingType_string =GlobalHelpers.getTeachingType(obj.TeachingType);

            // get user city
            
            obj.City_String = getCity(obj.TechearMemberShip.AppUser.Id);
            if (obj == null) return NotFound();

            return View(obj);
        }

        string getCity(string userId)
        {
            var cityId = _db.Users
            .Where(u=>u.Id ==  userId)
            .Include("City")
            .SingleOrDefault()?.City?.CityId;
            
            var city = _db.Cities.Find(cityId).EnName;

            return city;
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detail(string Approve, string reason, int Info, int PayExamFees, int PayLicFees, int TakeExam,
        int PassExam, int MemId, decimal LicFeesPrice, string ExamLocation, int TestId)
        {


            ModelState.Remove("Approve");
            ModelState.Remove("reason");
            ModelState.Remove("PayLicFees");
            ModelState.Remove("LicFeesPrice");
            ModelState.Remove("ExamLocation");

            string notifyBody = "";

            //wf history
            WFHistory wfHistory = new WFHistory();

            if (ModelState.IsValid)
            {
                string content = "";
                string subject = "SAUDI YOGA COMMITTEE";
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();

                var tech = _db.techearMemberShipTests
                .Include(t=>t.TechearMemberShip)
                .Include(t=>t.TechearMemberShip.AppUser)
                .Where(t=>t.TestId == TestId)
                .FirstOrDefault();

                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> { tech.TechearMemberShip.AppUser.Email },
                    Subject = subject,
                    Body = content
                };
                if (!string.IsNullOrEmpty(Approve))
                {
                    tech.RejectReason = "";
                    if (PayExamFees == 2 || PayLicFees == 2)
                    {
                        ModelState.AddModelError("", "You Must click reject button after type the reject reason");
                        tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                        return View(tech);
                    }
                    if (Info == 1)
                    {

                        tech.Status = (int)StatusEnum.Approved;
                        tech.ExamLocation = ExamLocation;
                        content = $"Congratulation, Your information is approved, next step is take the exam please go to your profile and pay the exam fees. </p>";
                        // wfhistory
                        wfHistory.Description = "Approve Information Confirmation";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.ApproveTeacherLicense_step1_BasicInformation;
                        notifyBody = "Approve Basic information";
                    }
                    if (PayExamFees == 1)
                    {
                        if (tech.Status == (int)StatusEnum.Pending)
                        {
                            ModelState.AddModelError("", "Information Must approved first before confirm the exam fees");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }
                        if (string.IsNullOrEmpty(ExamLocation) && PayExamFees == 1)
                        {
                            ModelState.AddModelError("", "Please Insert The Exam Location and Date!");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }
                        tech.PayExamFees = true;
                        content += "Your Exam Fees Approved, next step is take the exam at the below address.. ";
                        content += $" {ExamLocation}";
                        // update location in db
                        tech.ExamLocation = ExamLocation;

                        // wfhistory
                        wfHistory.Description = "Approve Pay Exam Fees";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.ApproveTeacherLicense_step2_PayExamFees;
                        notifyBody = "Approve pay exam fees.";
                    }

                    if (PayLicFees == 1)
                    {
                        if (tech.Status == (int)StatusEnum.Pending)
                        {
                            ModelState.AddModelError("", "Information Must approved first before confirm the license fees");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }
                        tech.PayFees = true;
                        tech.FinalApprove = true;
                        tech.ExpireDate = DateTime.Now.AddYears(1);
                        // Generate card serial
                        var serials = _db.techearMemberShipTests.Select(m => m.SerialNumber).ToList();
                        string serialNumber = YogaUtilities.GenerateSerialNumber(serials);
                        tech.SerialNumber = $"{tech.TestId}{serialNumber}";
                        string userImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\images",
                tech.TechearMemberShip.AppUser.UserImage);
                        
                        content += @$"<div>
                        <p>Congratulation, You have licensed SAUDI YOGA COMMITTEE Teacher</p>
                        </div>
                        <div style='text-align: center; width:200px;height: 270px; padding:30px;
    background-color: #efece5;color:#b77b57;font-family: 'Courier New', Courier, monospace;'>
        <div style='padding-bottom: 20px;'>
            <img width='80px' src='https://iili.io/r1zcZb.png'
            alt='Yoga'> 
        </div>
       <div >
        <div>
            {tech.TechearMemberShip.AppUser.FirstName} {tech.TechearMemberShip.AppUser.LastName}
        </div>
        <div>
            ID: {serialNumber}
        </div>
        <div>
            Validity: {DateTime.Now.AddYears(1).ToShortDateString()}
</div></div></div>
                        ";

                        //Generate PDF
                        string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\images", userImage);
                        string fileName = $"techer_licnese{tech.SerialNumber}.pdf";
                        var attachmentFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", fileName);


                        QuestDoc QD = new QuestDoc();
                        QD.GeneratePDFA5($"{tech.TechearMemberShip.AppUser.FirstName} {tech.TechearMemberShip.AppUser.LastName}", tech?.TechearMemberShip?.SerialNumber, attachmentFile, imgPath);

                        emailMessage.AttachmentFile = attachmentFile;
                        emailMessage.FileName = fileName;

                        // wfhistory
                        wfHistory.Description = "Approve Pay license Fees";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.ApproveTeacherLicense_step5_PayLicenceFees;
                        notifyBody =  "Approve Pay licence Fees";


                    }
                    if (TakeExam == 1)
                    {
                        if (tech.Status == (int)StatusEnum.Pending)
                        {
                            ModelState.AddModelError("", "Information Must approved first before take the exam");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }
                        content += "Thank you for taking the SAUDI YOGA COMMITTEE Teacher License  exam. ";
                        tech.TakeExam = true;

                        // wfhistory
                        wfHistory.Description = "Approve Take Exam";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.ApproveTeacherLicense_step3_TakeTheExam;
                        notifyBody = "Approve Take Exam";
                    }
                    if (PassExam == 1)
                    {
                        if (tech.Status == (int)StatusEnum.Pending)
                        {
                            ModelState.AddModelError("", "Information Must approved first before pass the exam");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }
                        if (LicFeesPrice <= 0)
                        {
                            ModelState.AddModelError("", "Please Insert The License Fees!");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }
                        content = "Congratulations, You Have Passed The SAUDI YOGA COMMITTEE Teacher licence exam. ";
                        tech.PassExam = true;
                        tech.LicenseFeesPrice = LicFeesPrice;

                        // wfhistory
                        wfHistory.Description = "Approve Pass Exam";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.ApproveTeacherLicense_step4_PassTheExam;
                        notifyBody = "Approve Pass the Exam";
                    }
                    



                }
                else
                {
                    if (PayExamFees == 2 && PayLicFees == 2 && Info == 2 && TakeExam == 2)
                    {
                        ModelState.AddModelError("", "You must reject only one step");
                        tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                        return View(tech);
                    }
                    if (PayExamFees == 2)
                    {
                        if (tech.Status == (int)StatusEnum.Pending)
                        {
                            ModelState.AddModelError("", "Information Must approved first before pass or not the exam");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }

                        content = $"Unfortunately, Your Exam fees payment not accepted {reason}";
                        tech.PassExam = false;
                        tech.ReceiptCopy = "";

                        // wfhistory
                        wfHistory.Description = "Reject Pay Exam Fees";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.RejectTeacherLicense_step2_2_PayExamFees;
                        notifyBody = "Reject Pay Exam Fees";
                    }
                    else if (PayLicFees == 2)
                    {
                        tech.PayFees = false;
                        tech.ReceiptCopyLic = "";
                        content = $"Unfortunately, Your license fees payment not accepted {reason}";

                        // wfhistory
                        wfHistory.Description = "Reject Pay License Fees";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.RejecteacherLicense_step5_5_PayLicenceFees;
                        notifyBody = "Reject Pay License Fees";
                    }
                    else if (Info == 2)
                    {
                        tech.Status = (int)StatusEnum.Rejected;
                        content = $"Sorry your data rejected for the below reason <p>{reason}</p>";
                        // wfhistory
                        wfHistory.Description = "Reject Information";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.RejectTeacherLicense_step1_1_BasicInformation;
                        notifyBody = "Reject Information";
                    }
                    
                    else if (PassExam == 2)
                    {
                        if (tech.Status == (int)StatusEnum.Pending)
                        {
                            ModelState.AddModelError("", "Information Must approved first before pass the exam");
                            tech.City_String = getCity(tech.TechearMemberShip.AppUser.Id);
                            return View(tech);
                        }

                        content = $"Unfortunately, You Did Not Pass The SAUDI YOGA COMMITTEE Teacher licence Exam. {reason}, You me re the exam again";
                        tech.PassExam = false;
                        tech.TakeExam = false;
                        tech.PayExamFees = false;
                        tech.ReceiptCopy = "";
                        tech.ReceiptCopyLic = "";
                        tech.LicenseFeesPrice = LicFeesPrice;
                        // wfhistory
                        wfHistory.Description = "Reject Pass Exam";
                        wfHistory.WFHistoryType = WFHistoryTypeEnum.RejectTeacherLicense_step4_4_PassTheExam;
                        notifyBody = "Reject Pass Exam";
                    }
                }


                tech.RejectReason = reason;
                _db.techearMemberShipTests.Update(tech);
                int rowAffect = _db.SaveChanges();
                // Send email
                var memUser = _db.Users.Where(u => u.Id == userId).FirstOrDefault();



                try
                {
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    emailMessage.Body = content;
                    if (rowAffect == 1)
                        _emailSender.SendEmailBySendGrid(emailMessage);

                    // Add notification
                    _notificationHelper.AddNotify(new Notification
                    {
                        AppUser = tech.TechearMemberShip.AppUser,
                        Body = content,
                        Title = notifyBody,
                        CreationDate = DateTime.Now,
                        IsRead = false,
                        AdminUser = loggedUser
                    });

                    // wf history
                    
                    wfHistory.AppUser = loggedUser;
                    wfHistory.CreationDate = DateTime.Now;
                    wfHistory.ModuleName = "TeacherLic";
                    wfHistory.RecordId = TestId;
                    int wfSaved = _wfHistoryManager.Save(wfHistory);



                    return RedirectToAction("Detail", "TeacherLic", new { id = TestId });
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tech = _db.techearMemberShipTests
            .Include(t=>t.TechearMemberShip)
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(m => m.TechearMemberShip.AppUser.Id == userId).FirstOrDefault();

            if (tech == null)
            {
                return NotFound();
            }

            vm.ExamDetails = tech != null ? tech.ExamLocation : "";

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayExamFees(IFormFile Receit, int TestId)
        {
            if (ModelState.IsValid)
            {
                string fileName_rec = "";
                if (Receit != null && Receit.Length > 0)
                {
                    fileName_rec = Path.GetFileName(Receit.FileName);
                    var filePath_rec = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_rec);
                    using (var fileSrteam = new FileStream(filePath_rec, FileMode.Create))
                    {
                        await Receit.CopyToAsync(fileSrteam);
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var tech = _db.techearMemberShipTests
                .Include(t=>t.TechearMemberShip)
                .Include(t=>t.TechearMemberShip.AppUser)
                .Where(t => t.TechearMemberShip.AppUser.Id == userId && t.TestId == TestId) 
                .FirstOrDefault();
                //tech.PayExamFees = true;
                tech.ReceiptCopy = fileName_rec;
                _db.techearMemberShipTests.Update(tech);
                int rowAffect = _db.SaveChanges();
                ViewData["Saved"] = "Thank you, We going to review your informatino ASAP.";

                try
                {
                    // Send Email
                    // get admin emails

                    List<string> adminEamils = getAdminEmails();

                    var emailMessage = new EmailMessage
                    {
                        ToEmailAddresses = adminEamils,
                        Subject = "Saudi Yoga Committe",
                        Body = "User has been payed the exam fees!"
                    };

                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);

                    if (rowAffect == 1)
                        _emailSender.SendEmailBySendGrid(emailMessage);

                    // Add notification
                    _notificationHelper.AddNotify(new Notification
                    {
                        AppUser = tech.TechearMemberShip.AppUser,
                        Body = emailMessage.Body,
                        Title = "Techer License - Pay exam fees",
                        CreationDate = DateTime.Now,
                        IsRead = false
                    });
                }
                catch (System.Exception ex)
                {

                    ModelState.AddModelError("Error, Please Try Again ", ex.Message);
                    return View(tech);
                }
                return RedirectToAction("Confirmation");
            }
            return View();
        }

        public ActionResult ConfirmPayExamFees()
        {
            return View();
        }

        public ActionResult Confirmation()
        {
            return View();
        }

        private List<string> getAdminEmails()
        {
            // Get a list of users in the role
            var usersWithPermission = _userManager.GetUsersInRoleAsync("Admin").Result;

            var users = usersWithPermission.OfType<AppUser>();

            List<string> emails = new List<string>();
            foreach (var item in users)
            {
                emails.Add(item.Email);
            }
            return emails;
        }

        public IActionResult PayLicFees()
        {
            TechearMemberShipVM vm = new TechearMemberShipVM();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tech = _db.techearMemberShipTests
            .Where(t => t.TechearMemberShip.AppUser.Id == userId)
            .FirstOrDefault();
            if (tech == null) return NotFound();
            vm.LicenseFeesPrice = tech.LicenseFeesPrice;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayLicFees(IFormFile Receit, int TestId)
        {
            if (ModelState.IsValid)
            {
                string fileName_rec = "";
                if (Receit != null && Receit.Length > 0)
                {
                    fileName_rec = Path.GetFileName(Receit.FileName);
                    var filePath_rec = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_rec);
                    using (var fileSrteam = new FileStream(filePath_rec, FileMode.Create))
                    {
                        await Receit.CopyToAsync(fileSrteam);
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var tech = _db.techearMemberShipTests
                .Include(t=>t.TechearMemberShip)
                .Where(t => t.TechearMemberShip.Id == userId && t.TestId == TestId)
                .FirstOrDefault();
                //tech.PayExamFees = true;
                tech.ReceiptCopyLic = fileName_rec;
                _db.techearMemberShipTests.Update(tech);
                int rowAffect = _db.SaveChanges();
                ViewData["Saved"] = "Thank you, We going to review your informatino ASAP.";
                try
                {
                    // Send Email
                    // get admin emails

                    List<string> adminEamils = getAdminEmails();

                    var emailMessage = new EmailMessage
                    {
                        ToEmailAddresses = adminEamils,
                        Subject = "Saudi Yoga Committe",
                        Body = "User has been payed the licence fees!"
                    };

                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);

                    if (rowAffect == 1)
                        _emailSender.SendEmailBySendGrid(emailMessage);

                    // Add notification
                    _notificationHelper.AddNotify(new Notification
                    {
                        AppUser = tech.TechearMemberShip.AppUser,
                        Body = emailMessage.Body,
                        Title = "Techer License - Pay licence fees",
                        CreationDate = DateTime.Now,
                        IsRead = false
                    });
                }
                catch (System.Exception ex)
                {

                    ModelState.AddModelError("Error, Please Try Again ", ex.Message);
                    return View(tech);
                }
                return RedirectToAction("Confirmation");
            }
            return View();
        }

        public IActionResult MemberData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _db.techearMemberShipTests
            .Include(t=>t.TechearMemberShip)
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(t => t.TechearMemberShip.Id == userId)
            .Select(t => new TechearMemberDataVM
            {
                FirstName = t.TechearMemberShip.AppUser.FirstName ,
                MiddleName = t.TechearMemberShip.AppUser.MiddleName,
                LastName = t.TechearMemberShip.AppUser.LastName,
                Phone = t.TechearMemberShip.AppUser.PhoneNumber,
                Email = t.TechearMemberShip.AppUser.Email,
                Nationality = t.TechearMemberShip.AppUser.Country.EnName,
                IssueDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.AddYears(-1).ToShortDateString() : "",
                ExpYears = t.TechearMemberShip.ExpYears,
                AccreditedHours = t.AccreditedHours,
                EducationLevel = getEducationLevel((int)t.TechearMemberShip.EducationLevel),
                TeachingType =GlobalHelpers.getTeachingType((int)t.TeachingType),
                PayExamFees = t.PayExamFees == true ? "Yes" : "No",
                PayLicFees = t.PayFees == true ? "Yes" : "No",
                Status = getCurrentStatus(t.Status),
                FinalApprove = t.FinalApprove == false ? "Pending" : "Approved",
                SerialNumber = string.IsNullOrEmpty(t.SerialNumber) ? "Not Generated Yet" : t.SerialNumber,
                ExpireDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.ToShortDateString() : "",
                SchoolSocialMediaAccount = t.SchoolSocialMediaAccount,
                PersonalWebSite = t.TechearMemberShip.PersonalWebSite,
                SchoolLocation = t.SchoolLocation,
                CertaficateDate = t.CertaficateDate.ToShortDateString(),
                SchoolName = t.SchoolName,
                SchoolLink = t.SchoolLink,
                SocialMediaAccounts = t.TechearMemberShip.SocialMediaAccounts,
                CertficateFiles = t.CertficateFiles
            })
            .FirstOrDefault();

            return View(result);
        }

        [HttpPost]
        public IActionResult MemberData(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _db.techearMemberShipTests
            .Include(t=>t.TechearMemberShip)
            .Include(t=>t.TechearMemberShip.AppUser)
            .Where(t => t.TechearMemberShip.Id == userId)
            .Select(t => new TechearMemberDataVM
            {
                FirstName = t.TechearMemberShip.AppUser.FirstName ,
                MiddleName = t.TechearMemberShip.AppUser.MiddleName,
                LastName = t.TechearMemberShip.AppUser.LastName,
                Phone = t.TechearMemberShip.AppUser.PhoneNumber,
                Email = t.TechearMemberShip.AppUser.Email,
                Nationality = t.TechearMemberShip.AppUser.Country.EnName,
                IssueDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.AddYears(-1).ToShortDateString() : "",
                ExpYears = t.TechearMemberShip.ExpYears,
                AccreditedHours = t.AccreditedHours,
                EducationLevel = getEducationLevel((int)t.TechearMemberShip.EducationLevel),
                TeachingType =GlobalHelpers.getTeachingType((int)t.TeachingType),
                PayExamFees = t.PayExamFees == true ? "Yes" : "No",
                PayLicFees = t.PayFees == true ? "Yes" : "No",
                Status = getCurrentStatus(t.Status),
                FinalApprove = t.FinalApprove == false ? "Pending" : "Approved",
                SerialNumber = string.IsNullOrEmpty(t.SerialNumber) ? "Not Generated Yet" : t.SerialNumber,
                ExpireDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.ToShortDateString() : "",
                SchoolSocialMediaAccount = t.SchoolSocialMediaAccount,
                PersonalWebSite = t.TechearMemberShip.PersonalWebSite,
                SchoolLocation = t.SchoolLocation,
                CertaficateDate = t.CertaficateDate.ToShortDateString(),
                SchoolName = t.SchoolName,
                SchoolLink = t.SchoolLink,
                SocialMediaAccounts = t.TechearMemberShip.SocialMediaAccounts,
                CertficateFiles = t.CertficateFiles
            })
            .FirstOrDefault();


            // Generete pdf file
            //1var Rendered = new ChromePdfRenderer(); 
            string htmlContent = $@"<div><div><div><h2>Teacher Licence Application form</h2><div><div><div><label><b>First Name</b>
            </label><label>{result.FirstName}</label></div><div><label><b>Middle Name</b></label><label>{result.MiddleName}</label>
            </div><div><label><b>LastName</b></label><label>{result.LastName}</label></div><div><label><b>Education Level</b>
            </label><label>{result.EducationLevel}</label></div><div><label><b>Social Media Accounts</b></label><label>
            {result.SocialMediaAccounts}</label></div><div><label><b>Personal WebSite</b></label><label>
            {result.PersonalWebSite}</label></div><div><label><b>Teaching Type</b></label><label>
            {result.TeachingType}</label></div></div><div><div><label><b>Years of experience</b></label><label>{result.ExpYears}</label>
            </div><div><label><b>Accredited Hours</b></label><label>{result.AccreditedHours}</label></div><div>
            <label><b>School Location</b></label><label>{result.SchoolLocation}</label></div><div><label>
            <b>Certaficate Date</b></label><label>{result.CertaficateDate}</label></div><div><label><b>School Name</b>
            </label><label>{result.SchoolName}</label></div><div><label><b>School Link</b></label><label>{result.SchoolLink}
            </label></div><div><label><b>School Social Media Account</b></label><label>{result.SchoolSocialMediaAccount}
            </label></div></div></div></div></div></div></div></div></div></div>";
            //2using var PDF = Rendered.RenderHtmlAsPdf(htmlContent);
            string pdfName = $"Techers_Licenses_data_{result.FirstName}{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf";

            //3 return new FileStreamResult(PDF.Stream, "application/pdf")
            // {
            //     FileDownloadName = pdfName
            // };

            return View();
        }

        public IActionResult ExportToExcel(int Id)
        {
            
            // var result = _db.techearMemberShipTests
            // .Include("TechearMemberShip")
            // .Include(t=>t.TechearMemberShip.AppUser)
            // .Where(m=>m.TechearMemberShip.MemId == Id)
            // .ToList();

            var result = _db.techearMemberShipTests
            .Include("TechearMemberShip")
            .Select(t => new
            {
                UserId = t.TestId,
                FirstName = t.TechearMemberShip.AppUser.FirstName,
                MiddleName = t.TechearMemberShip.AppUser.MiddleName,
                LastName = t.TechearMemberShip.AppUser.LastName,
                Phone = t.TechearMemberShip.AppUser.PhoneNumber,
                Email = t.TechearMemberShip.AppUser.Email,
                Nationality = t.TechearMemberShip.AppUser.Country.EnName,
                City = t.TechearMemberShip.AppUser.City.EnName,
                IssueDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.AddYears(-1).ToShortDateString() : "",
                ExperienceYears = t.TechearMemberShip.ExpYears,
                AccreditedHours = t.AccreditedHours,
                EducationLevel = getEducationLevel((int)t.TechearMemberShip.EducationLevel),
                TeachingType = getTeachingType((int)t.TeachingType),
                PayExamFees = t.PayExamFees == true ? "Paid" : "Not Paid",
                PayLicFees = t.PayFees == true ? "Paid" : "Not Paid",
                Status = getCurrentStatus(t.Status),
                PassExam = t.PassExam == true ? "Yes" : "No",
                FinalApprove = t.FinalApprove == false ? "Pending" : "Approved",
                CurrentInformationStatus = getCurrentStatus(t.Status),
                SerialNumber = string.IsNullOrEmpty(t.SerialNumber) ? "Not Generated Yet" : t.SerialNumber,
                ExpireDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.ToShortDateString() : ""
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
            if (statusId == 1) return "Pending";
            if (statusId == 2) return "Approved";
            if (statusId == 3) return "Rejected";
            return "Pending";
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var member = IfTeacherExists(Id);
            if (member == null) throw new Exception("Teacher Not Found!");
            var vm = new TechearMemberShip_EditVM();
            vm.EducationLevels = getEducationLevels();
            vm.TeachingTypes = getTeachingTypes();
            vm.AccreditedHours = member.AccreditedHours;
            vm.CertaficateDate = member.CertaficateDate;
            vm.CertficateFiles = member.CertficateFiles;
            vm.EducationLevel = (int)member.TechearMemberShip.EducationLevel;
            vm.ExamDetails = !string.IsNullOrEmpty(member.ExamLocation) ? member.ExamLocation : "";
            vm.ExpYears = member.TechearMemberShip.ExpYears;
            vm.PersonalWebSite = member.TechearMemberShip.PersonalWebSite;
            vm.SchoolLink = member.SchoolLink;
            vm.SchoolLocation = member.SchoolLocation;
            vm.SchoolName = member.SchoolName;
            vm.SchoolSocialMediaAccount = member.SchoolSocialMediaAccount;
            vm.SocialMediaAccounts = member.SchoolSocialMediaAccount;
            vm.TeachingType = member.TeachingType;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TechearMemberShip_EditVM obj, IFormFile CertficateFiles)
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
            ModelState.Remove("Image");
            obj.Name = "tst";

            var member = IfTeacherExists(int.Parse(RouteData.Values["Id"].ToString()));

            // if(obj.TeachingType == 0)
            // {
            //     obj.EducationLevels = getEducationLevels();
            //     obj.TeachingTypes = getTeachingTypes();
            //     ModelState.AddModelError("TeachingType", " Teaching Type is required");
            //     return View(obj);
            // }
            // if(obj.EducationLevel == 0)
            // {
            //     obj.EducationLevels = getEducationLevels();
            //     obj.TeachingTypes = getTeachingTypes();
            //     ModelState.AddModelError("EducationLevel", " Education Level is required");
            //     return View(obj);
            // }CertficateFiles != null && CertficateFiles.Count > 0
            if (string.IsNullOrEmpty(member.CertficateFiles) && CertficateFiles == null)
            {
                obj.EducationLevels = getEducationLevels();
                obj.TeachingTypes = getTeachingTypes();
                ModelState.AddModelError("CertficateFiles", " Certficate Files required");
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                // Upload images
                string fileName_rec = "";
                string fileName_cert = "";


                if (CertficateFiles != null && CertficateFiles.Length > 0)
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
                var loggedUser = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (loggedUser == null)
                {
                    ModelState.AddModelError("", "User Not Exist");
                    return View(obj);
                }

                var entity = _db.techearMemberShipTests
                .Include(t=>t.TechearMemberShip)
                .Where(m => m.TestId == member.TestId)
                .FirstOrDefault();

                

                entity.TechearMemberShip.EducationLevel = (EducationLevelEnum)obj.EducationLevel;
                entity.TechearMemberShip.SocialMediaAccounts = obj.SocialMediaAccounts;
                entity.TechearMemberShip.PersonalWebSite = obj.PersonalWebSite;
                entity.TeachingType = obj.TeachingType.Value;
                entity.TechearMemberShip.ExpYears = obj.ExpYears.Value;
                entity.AccreditedHours = obj.AccreditedHours.Value;
                entity.SchoolLocation = obj.SchoolLocation;
                entity.CertaficateDate = obj.CertaficateDate.Value;
                entity.SchoolName = obj.SchoolName;
                entity.SchoolLink = obj.SchoolLink;
                entity.SchoolSocialMediaAccount = obj.SchoolSocialMediaAccount;
                entity.CertficateFiles = string.IsNullOrEmpty(CertficateFiles?.FileName) ? "" : CertficateFiles.FileName;
                entity.TechearMemberShip.Name = obj.Name;
                entity.Status = (int)StatusEnum.Pending;

                _db.techearMemberShipTests.Update(entity);
                int rowAffect = _db.SaveChanges();
                // add notification
                if (rowAffect == 1 || rowAffect > 1)
                {
                    _notificationHelper.AddNotify(new Notification
                    {
                        AppUser = loggedUser,
                        Body = "User Re Submitted his rejected rquest.",
                        Title = "Re Submit",
                        CreationDate = DateTime.Now,
                        IsRead = false
                    });
                }
                return RedirectToAction("DataSaved");
            }
            obj.EducationLevels = getEducationLevels();
            obj.TeachingTypes = getTeachingTypes();
            return View(obj);
        }
    }
}