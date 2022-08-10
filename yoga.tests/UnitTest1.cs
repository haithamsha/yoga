using System.Collections.Generic;
using System.Linq;
using Xunit;

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
}