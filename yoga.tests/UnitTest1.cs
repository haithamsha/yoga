using System.Collections.Generic;
using System.Linq;
using Mailjet.Client;
using Xunit;
using yoga.Models;
using Newtonsoft.Json.Linq;
using Mailjet.Client.Resources;
using System.Threading.Tasks;
using Mailjet.Client.TransactionalEmails;

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

        var result = serials.Where(s => s == serial);

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

        var result = serials.Where(s => s == serial);

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
            ToEmailAddresses = new List<string> { "haithamshaabann@gmail.com", "haithamshaabann@gmail.com" },
            Subject = "Test with image2",
            Body = content
        };
        _emailSender.SendEmailBySendGrid(emailMessage);
    }

    [Fact]
    public async Task SendEmailByMailJetAsync()
    {
        MailjetClient client = new MailjetClient("1a939e1a809792b94a1420d96aaf7de8", "9ff85944ea8b42d39c60ff60f39fd3a6");

        MailjetRequest request = new MailjetRequest
        {
            Resource = Send.Resource
        };

        // construct your email with builder
        var email = new TransactionalEmailBuilder()
               .WithFrom(new SendContact("shahaitham@gmail.com"))
               .WithSubject("Test subject")
               .WithHtmlPart("<h1>Header</h1>")
               .WithTo(new SendContact("haithamshaabann@gmail.com"))
               .Build();

        // invoke API to send email
        var response = await client.SendTransactionalEmailAsync(email);

        // check response
        Assert.Equal(1, response.Messages.Length);
    }

}