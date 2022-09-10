namespace yoga.Models
{
    public static class YogaUtilities
    {

        public static string GenerateSerialNumber(List<string>? serials) 
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            string serialNumber = "";
            if(r.Distinct().Count() == 1)
            {
                serialNumber = GenerateSerialNumber(serials);
            }
            else {
                serialNumber = r;
            }

            // Check if this serial number exsiting in database
            var dbSerials = serials;
            
            if(dbSerials == null || dbSerials.Count() == 0) return serialNumber;

            var result = dbSerials.Where(s=>s == serialNumber);
            if(result == null || result.Count() == 0) return serialNumber;
            
            GenerateSerialNumber(serials);
            return "";
        }
    }
}