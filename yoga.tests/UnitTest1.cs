using System.Collections.Generic;
using System.Linq;
using Xunit;
using yoga.Models;

namespace yoga.tests;

public class UnitTest1
{
    [Fact]
    public void IsRandomNumberExists()
    {
        List<string> serials = new List<string>()
        {
            "123456"
        };
        string serial = yoga.Models.YogaUtilities.GenerateSerialNumber(serials);

        var result = serials.Where(s=>s == serial);
        
        Assert.Null(result);
    }

    [Fact]
    public void SendEmail()
    {
        List<string> serials = new List<string>()
        {
            "123456"
        };
        string serial = yoga.Models.YogaUtilities.GenerateSerialNumber(serials);

        var result = serials.Where(s=>s == serial);
        
        Assert.Null(result);
    }

    [Fact]
    public void SendEmailWithImage()
    {

        //Assert.Null();
        string content = @$"<div>
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
            <img width='80px' src='https://iili.io/r1uyHN.png' alt='Yoga'>
        </div>
       <div >
        <div>
            Haitham AbdElRady
        </div>
        <div>
            ID: 0000000
        </div>
        <div>
            Validity: 2233022
</div></div></div>
                        ";
        EmailConfiguration _emailConfiguration = new EmailConfiguration();
        EmailSender _emailSender = new EmailSender(_emailConfiguration);
        var emailMessage = new EmailMessage
                {
                    ToEmailAddresses = new List<string> {"rokapokka@gmail.com"},
                    Subject = "Test with image",
                    Body = content
                };
        _emailSender.SendEmailBySendGrid(emailMessage);
    }
}