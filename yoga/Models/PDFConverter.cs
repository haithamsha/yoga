using PuppeteerSharp;

namespace yoga.Models
{
    public class PDFConverter
    {
        public async Task<int> GeneratePdfFile(string htmlContent, string attachmentFile)
        {
            //var Rendered = new ChromePdfRenderer();
            //using var PDF = Rendered.RenderHtmlAsPdf(htmlContent);

            //PDF.SaveAs(attachmentFile);

            //string pdfPath = attachmentFile;
            //using (PdfWriter writer = new PdfWriter(pdfPath))
            //{
            //    using (PdfDocument pdf = new PdfDocument(writer))
            //    {
            //        Document document = new Document(pdf);
            //        document.Add(new Paragraph(htmlContent));
            //        document.Close();
            //    }
            //}


            try
            {
                // Specify the Chromium revision if necessary
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();

                // Launch a headless browser
                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                using var page = await browser.NewPageAsync();

                // Define the HTML content
            //    string htmlContent = @"
            //<html>
            //<head>
            //    <style>
            //        body { font-family: Arial, sans-serif; margin: 20px; }
            //        h1 { color: blue; }
            //        p { font-size: 18px; }
            //    </style>
            //</head>
            //<body>
            //    <h1>Invoice</h1>
            //    <p>Client: John Doe</p>
            //    <p>Product: Example Product</p>
            //    <p>Price: $100</p>
            //</body>
            //</html>";

                // Set the HTML content
                await page.SetContentAsync(htmlContent);

                // Define the output PDF file path
                string pdfFilePath = attachmentFile;

                // Generate the PDF
                await page.PdfAsync(pdfFilePath, new PdfOptions
                {
                    Format = PuppeteerSharp.Media.PaperFormat.A4,
                    PrintBackground = true
                });

                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1;
            }

        }
    }
}
