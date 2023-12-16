using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using yoga.Data;
using yoga.Helpers;
using yoga.Models;
using yoga.ViewModels;

namespace yoga.Controllers
{
    public class MembershipCardController: Controller
    {
        private readonly YogaAppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        private readonly NotificationHelper _notificationHelper;

        private readonly IWFHistoryManager _wfHistoryManager;
        // private readonly IEmailSender _emailSender;
        public MembershipCardController(YogaAppDbContext db, 
        UserManager<AppUser> userManager, IWFHistoryManager wfHistoryManager
        )
        {
            _db = db;
            _userManager = userManager;
            _notificationHelper = new NotificationHelper(_db);
            _wfHistoryManager = wfHistoryManager;
        }

        public IActionResult Index()
        {
            IEnumerable<MembershipCard> cards = _db.MembershipCards.Include("AppUser");
            return View(cards);
        }
        
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(MembershipCardVM obj, IFormFile RecietCopy, IFormFile Image, int id)
        {
            ModelState.Remove("RecietCopy");
            ModelState.Remove("Image");
            if(ModelState.IsValid)
            {
                // Upload image
                string fileName = "";
                if(RecietCopy == null || RecietCopy.Length == 0)
                {
                    ModelState.AddModelError("RecietCopy", "Reciet Copy Is required");
                    return View(obj);
                }
                if(RecietCopy != null && RecietCopy.Length > 0)
                {
                    fileName = Path.GetFileName(RecietCopy.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName);
                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await RecietCopy.CopyToAsync(fileSrteam);
                    }
                }

                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
                if(loggedUser == null)
                {
                    ModelState.AddModelError("", "User Not Exist");
                    return View();
                }

                // validate user image
                if(string.IsNullOrEmpty(loggedUser.UserImage))
                {
                    // Upload image
                    string fileName_image = "";
                    if(Image != null && Image.Length > 0)
                    {
                        fileName_image = Path.GetFileName(Image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", fileName_image);
                        using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileSrteam);
                        }
                        // update user image column
                        var user = _db.Users.Find(loggedUser.Id);
                        if(user != null)
                        {
                            user.UserImage = fileName_image;
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


                if(id > 0)
                {
                    var entity_forUpdate = _db.MembershipCards.Find(id);
                    entity_forUpdate.RecietCopy = fileName;
                    entity_forUpdate.Status = (int)StatusEnum.Pending;
                    _db.MembershipCards.Update(entity_forUpdate);
                }
                else
                {
                    MembershipCard entity_forCreate = new MembershipCard
                    {
                        AppUser = loggedUser,
                        RecietCopy = fileName
                    };

                    _db.MembershipCards.Add(entity_forCreate);
                }
                

                await _db.SaveChangesAsync();

                // Add user to member role
                await _userManager.AddToRoleAsync(loggedUser, "Member");
                
                ViewData["Saved"] = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";
                EmailConfiguration _emailConfiguration = new EmailConfiguration();
                EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    
                string content = $"Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";
                
                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {loggedUser.Email},
                    Subject = "SAUDI YOGA COMMITTEE - MemberShip Card Rquest",
                    Body = content
                };

                // Generate pdf license

                // Add notification
                
                _notificationHelper.AddNotify(new Notification
                {
                    AppUser = loggedUser,
                    Body = content,
                    Title = "Create Membership Card",
                    CreationDate = DateTime.Now,
                    IsRead = false
                });
                
                _emailSender.SendEmailBySendGrid(emailMessage);

                ViewData["Saved"] = "Your request has been sent successfully. Our team will review it and approve it as soon as possible. Thank you.";
                return RedirectToAction("DataSaved");
            }
            return View();
        }
        public IActionResult DataSaved()
        {
            return View();
        }

        public IActionResult Detail(int? id)
        {
            if(id == null || id == 0) return NotFound();
            var obj = _db.MembershipCards.Include("AppUser").Where(m=>m.CardId == id).SingleOrDefault();
            if(obj == null) return NotFound();

            return View(obj);
        }

        // //Post
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Detail(MembershipCard obj)
        // {
        //     if(ModelState.IsValid)
        //     {
        //         _db.MembershipCards.Update(obj);
        //         _db.SaveChanges();
        //         return View(obj);
        //     }
        //     return View();
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detail(int id, string Approve, string reason)
        {
            if(!string.IsNullOrEmpty(Approve))
            {
                var memCard = _db.MembershipCards.Include("AppUser").Where(m=>m.CardId == id).SingleOrDefault();
                // Update status, payed, active and expire date for 1 year
                memCard.Active = true;
                memCard.Payed = true;
                memCard.ExpireDate = DateTime.Now.AddYears(1);
                memCard.Status = (int)StatusEnum.Approved;
                
                // Generate card serial
                var serials = _db.MembershipCards.Select(m=>m.SerialNumber).ToList();
                string serialNumber = YogaUtilities.GenerateSerialNumber(serials);
                memCard.SerialNumber = $"{memCard.CardId}{serialNumber}";
                _db.MembershipCards.Update(memCard);
                int rowAffect = _db.SaveChanges();
                // Send Email to the user(Congratuilation, Your Membership card is active now until {expire date}, Your)
                // Card Serial Number "999"
                string userImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets\\images", 
                memCard.AppUser.UserImage);

                string content = @$"<div>
                        <p>
                        Congratulations, Your SAUDI YOGA COMMITTEE Membership Card Is Now Active.
                        </p>
                        </div>
                        <div style='text-align: center; width:200px;height: 270px; padding:30px;
    background-color: #efece5;color:#b77b57;font-family: 'Courier New', Courier, monospace;'>
        <div style='padding-bottom: 20px;'>
            <img width='80px' src='https://iili.io/r1zcZb.png'
            alt='Yoga'> 
        </div>
       <div >
        <div>
            {memCard.AppUser.FirstName} {memCard.AppUser.LastName}
        </div>
        <div>
            ID: {serialNumber}
        </div>
        <div>
            Validity: {DateTime.Now.AddYears(1)}
</div></div></div>
                        ";

                        string htmlContent = @$"<div>
                        
                        </div>
                        <div style='text-align: center; width:200px;height: 270px; padding:30px;
    background-color: #efece5;color:#b77b57;font-family: 'Courier New', Courier, monospace;'>
        <div style='padding-bottom: 20px;'>
            <img width='80px' src='https://iili.io/r1zcZb.png'
            alt='Yoga'> 
        </div>
        <div>
            <img width='80px' height='80px' src='{userImage}' alt='Yoga'>
        </div>
       <div >
        <div>
            {memCard.AppUser.FirstName} {memCard.AppUser.LastName}
        </div>
        <div>
            ID: {serialNumber}
        </div>
        <div>
            Validity: {DateTime.Now.AddYears(1)}
</div></div></div>
                        ";
                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();

                //Generate pdf file
                //1var Rendered = new ChromePdfRenderer(); 
                //2using var PDF = Rendered.RenderHtmlAsPdf(htmlContent);
                string PdffileName = $"MemberShip_Card{memCard.SerialNumber}.pdf";
                var attachmentFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", PdffileName);
                //3PDF.SaveAs(attachmentFile);

                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {memCard.AppUser.Email},
                    Subject = "SAUDI YOGA COMMITTEE",
                    Body = content,
                    //4AttachmentFile = attachmentFile,
                    //5FileName = PdffileName
                };

                try
                {
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    if(rowAffect == 1)
                    _emailSender.SendEmailBySendGrid(emailMessage);

                    // add notification
                    _notificationHelper.AddNotify(new Notification
                    {
                        AppUser = memCard.AppUser,
                        Body = "Congratulations, Your SAUDI YOGA COMMITTEE Membership Card Is Now Active",
                        CreationDate = DateTime.Now,
                        IsRead = false,
                        Title = "Membership Card Approved"
                    });

                    // add wfhistory
                    WFHistory wfHistory = new WFHistory();
                    wfHistory.AppUser = loggedUser;
                    wfHistory.WFHistoryType = WFHistoryTypeEnum.ApproveMembership;
                    wfHistory.RecordId = id;
                    wfHistory.CreationDate = DateTime.Now;
                    wfHistory.ModuleName = "Membership";
                    wfHistory.Description = "Approve membership card";
                    int wfSaved = _wfHistoryManager.Save(wfHistory);
                    return RedirectToAction("Index", "MembershipCard");
                }
                catch (System.Exception ex)
                {
                    var obj = _db.MembershipCards.Include("AppUser").Where(m=>m.CardId == id).SingleOrDefault();
                    ModelState.AddModelError("", ex.Message);
                    return View(obj);
                }
            }
            else 
            {
                // Update status to rejected
                var memCard = _db.MembershipCards.Include("AppUser").Where(m=>m.CardId == id).SingleOrDefault();
                memCard.Status = (int)StatusEnum.Rejected;
                memCard.Active = false;
                memCard.ExpireDate = null;
                _db.MembershipCards.Update(memCard);
                int rowAffect = _db.SaveChanges();
                // Send Email to the user.
                string content = $"We Are Sorry Your Request Rejected, becuase {reason}";
                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {memCard.AppUser.Email},
                    Subject = "SAUDI YOGA COMMITTEE",
                    Body = content
                };

                try
                {
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    if(rowAffect == 1) _emailSender.SendEmailBySendGrid(emailMessage);
                     // add notification
                    _notificationHelper.AddNotify(new Notification
                    {
                        AppUser = memCard.AppUser,
                        Body = content,
                        CreationDate = DateTime.Now,
                        IsRead = false,
                        Title = "Membership Card Rejection"
                    });

                    // add wfhistory
                    WFHistory wfHistory = new WFHistory();
                    wfHistory.AppUser = loggedUser;
                    wfHistory.WFHistoryType = WFHistoryTypeEnum.RejectMembership;
                    wfHistory.RecordId = id;
                    wfHistory.CreationDate = DateTime.Now;
                    wfHistory.ModuleName = "Membership";
                    wfHistory.Description = "Reject membership card";
                    int wfSaved = _wfHistoryManager.Save(wfHistory);

                    return RedirectToAction("Index", "MembershipCard");
                }
                catch (System.Exception ex)
                {
                    
                    ModelState.AddModelError("", ex.Message);
                }
                return View();
            }
            
        }

        public IActionResult ExportToExcel()
        {
            var result = _db.MembershipCards
            .Select( t => new {
                UserId = t.CardId,
                FirstName = t.AppUser.FirstName,
                MiddleName = t.AppUser.MiddleName,
                LastName = t.AppUser.LastName,
                Nationality = t.AppUser.Country.EnName,
                Phone = t.AppUser.PhoneNumber,
                Email= t.AppUser.Email,
                CardSerial = string.IsNullOrEmpty(t.SerialNumber) ? "Not Generated Yet" : t.SerialNumber,
                IssueDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.AddYears(-1).ToShortDateString(): "",
                ExpireDate = t.ExpireDate.HasValue == true ? t.ExpireDate.Value.ToShortDateString() : "",
                PayFees = t.Payed  == true ? "Yes": "No",
                Status = getCurrentStatus(t.Status)
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
            string excelName = $"Membership cards {DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        static string getCurrentStatus(int statusId)
        {
            if(statusId == 1) return "Pending";
            if(statusId == 2) return "Approved";
            if(statusId == 3) return "Rejected";
            return "Pending";
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Reject(int id, string reason)
        // {

        //     // Update status to rejected
        //     var memCard = _db.MembershipCards.Where(m=>m.CardId == id).SingleOrDefault();
        //     memCard.Status = (int)StatusEnum.Rejected;
        //     // Send Email to the user.
        //     string content = $"We Are Sorry Your Request Rejected, becuase {reason}";
        //     string userId = _userManager.GetUserId(User);
        //     var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
        //     var emailMessage = new EmailMessage
        //     {
        //         ToEmailAddresses = new List<string> {loggedUser.Email},
        //         Subject = "SAUDI YOGA COMMITTEE",
        //         Body = content
        //     };

        //     try
        //     {
        //         EmailConfiguration _emailConfiguration = new EmailConfiguration();
        //         EmailSender _emailSender = new EmailSender(_emailConfiguration);
        //         _emailSender.SendEmail(emailMessage);
        //     }
        //     catch (System.Exception ex)
        //     {
                
        //         ModelState.AddModelError("", ex.Message);
        //     }
        //     return View();
        // }


        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}