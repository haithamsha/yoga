using System;
using System.IO;



namespace yoga.Models
{
    public static class YogaUtilities
    {

        public static string GenerateSerialNumber(List<string>? serials) 
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D5");
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

        public static void GeneratePdfFile(string htmlContent, string attachmentFile)
        {
            //string pdfPath = "MyPDF.pdf";
            //using (PdfWriter writer = new PdfWriter(attachmentFile))
            //{
            //    using (PdfDocument pdf = new PdfDocument(writer))
            //    {
            //        Document document = new Document(pdf);
            //        document.Add(new Paragraph(htmlContent));
            //        document.Close();
            //    }
            //}

           


            //string htmlFilePath = $"{attachmentFile}\\temp.html";
            //System.IO.File.WriteAllText(htmlFilePath, htmlContent);

            //// Define the output PDF file path
            //string pdfFilePath = $"{attachmentFile}\\invoice.pdf";

            //// Use wkhtmltopdf to convert HTML to PDF
            //ProcessStartInfo processStartInfo = new ProcessStartInfo
            //{
            //    FileName = pdfFilePath,
            //    Arguments = $"{htmlFilePath} {pdfFilePath}",
            //    RedirectStandardOutput = true,
            //    UseShellExecute = false,
            //    CreateNoWindow = true
            //};

            //Process process = new Process { StartInfo = processStartInfo };
            //process.Start();
            //process.WaitForExit();

            //// Clean up the temporary HTML file
            //System.IO.File.Delete(htmlFilePath);



            //quest

        }
    }
}