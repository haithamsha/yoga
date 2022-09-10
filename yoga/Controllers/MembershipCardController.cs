using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using yoga.Data;
using yoga.Models;
using yoga.ViewModels;

namespace yoga.Controllers
{
    public class MembershipCardController: Controller
    {
        private readonly YogaAppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        // private readonly IEmailSender _emailSender;
        public MembershipCardController(YogaAppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
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
        public async Task<IActionResult> Create(MembershipCardVM obj, IFormFile RecietCopy)
        {
            ModelState.Remove("RecietCopy");
            if(ModelState.IsValid)
            {
                // Upload image
                string fileName = "";
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


                MembershipCard entity = new MembershipCard{
                    AppUser = loggedUser,
                    RecietCopy = fileName
                };


                _db.MembershipCards.Add(entity);

                await _db.SaveChangesAsync();

                // Add user to member role
                await _userManager.AddToRoleAsync(loggedUser, "Member");
                
                ViewData["Saved"] = "Thanks, Your request sent successfully, we will review your it and approve your card ASAP.";
                return View(obj);
            }
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
                var memCard = _db.MembershipCards.Where(m=>m.CardId == id).SingleOrDefault();
                // Update status, payed, active and expire date for 1 year
                memCard.Active = true;
                memCard.Payed = true;
                memCard.ExpireDate = DateTime.Now.AddYears(1);
                memCard.Status = (int)StatusEnum.Approved;
                
                // Generate card serial
                var serials = _db.MembershipCards.Select(m=>m.SerialNumber).ToList();
                string serialNumber = YogaUtilities.GenerateSerialNumber(serials);
                memCard.SerialNumber = serialNumber;
                _db.MembershipCards.Update(memCard);
                int rowAffect = _db.SaveChanges();
                // Send Email to the user(Congratuilation, Your Membership card is active now until {expire date}, Your)
                // Card Serial Number "999"
                string content = @$"<div>
                        <p>
                        Congratuilation, Your SAUDI YOGA COMMITTEE Membership Card Is Now Active.
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
                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {loggedUser.Email},
                    Subject = "SAUDI YOGA COMMITTEE",
                    Body = content
                };

                try
                {
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    if(rowAffect == 1)
                    _emailSender.SendEmailAsync(emailMessage);
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
                var memCard = _db.MembershipCards.Where(m=>m.CardId == id).SingleOrDefault();
                memCard.Status = (int)StatusEnum.Rejected;
                memCard.Active = false;
                _db.MembershipCards.Update(memCard);
                int rowAffect = _db.SaveChanges();
                // Send Email to the user.
                string content = $"We Are Sorry Your Request Rejected, becuase {reason}";
                string userId = _userManager.GetUserId(User);
                var loggedUser = _db.Users.Where(u=>u.Id == userId).FirstOrDefault();
                var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {loggedUser.Email},
                    Subject = "SAUDI YOGA COMMITTEE",
                    Body = content
                };

                try
                {
                    EmailConfiguration _emailConfiguration = new EmailConfiguration();
                    EmailSender _emailSender = new EmailSender(_emailConfiguration);
                    if(rowAffect == 1) _emailSender.SendEmailAsync(emailMessage);
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
                Name = t.AppUser.FirstName + " " + t.AppUser.LastName,
                Phone = t.AppUser.PhoneNumber,
                Email= t.AppUser.Email,
                CardSerial = t.SerialNumber,
                PayFees = t.Payed  == true ? "Yes": "No",
                Active = t.Status == 1  ? "Pending" : "Approved"
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