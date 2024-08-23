using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;



namespace yoga.Models
{
    public class QuestDoc
    {
        public void GeneratePDF(string htmlContent, string attachmentFile, string imgPath, string logo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Inch);
                    
                    page.Header()
                    .Text("SAUDI YOGA COMMITTEE")
                    .SemiBold().FontSize(20).FontColor(Colors.Green.Medium);

                    page.Content().Width(5, Unit.Centimetre).Height(5, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Item().Image(logo).FitWidth();
                        x.Item().Image(imgPath).FitWidth();
                        x.Item().Text("haitham abdelrady").AlignCenter().FontColor(Colors.Green.Medium);
                        x.Item().Text("ID: 123456").AlignCenter().FontColor(Colors.Green.Medium);
                        x.Item().Text(DateTime.Now.AddYears(1).ToShortDateString()).AlignCenter().FontColor(Colors.Green.Medium);
                    });

                    page.Footer()
                    .AlignCenter();
                    //.Text(x =>
                    //{
                    //    x.Span("Page ");
                    //    x.CurrentPageNumber();
                    //});
                });
            })
            .GeneratePdf(attachmentFile);
        }

        public void GeneratePDFA5(string name, string id, string attachmentFile, string imgPath)
        {
            string logo =  Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images", "yogalogoforpdf.png");

            QuestPDF.Settings.License = LicenseType.Community;
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    //page.DefaultTextStyle(x => x.FontFamily("Calibri").FontSize(20));

                    //page.Header()
                    //.Text("SAUDI YOGA COMMITTEE")
                    //.SemiBold().FontSize(20).FontColor(Colors.Green.Medium);

                    page.Content().AlignCenter().AlignMiddle()
                    .Column(column =>
                    {
                        //column.Spacing(20);
                        //column.Item()
                        //.Text("SAUDI YOGA COMMITTEE");
                        //.FontSize(32).FontColor(Colors.Blue.Darken2).SemiBold();

                        column.Item().Width(2.5f, Unit.Inch).Height(2.5f, Unit.Inch)
                        .Image(logo).UseOriginalImage();

                        column.Item().Width(2.5f, Unit.Inch).Height(2.5f, Unit.Inch).Image(imgPath).UseOriginalImage();
                        column.Spacing(5);
                    column.Item().Text(name).AlignCenter().FontColor(Colors.Green.Medium);
                        column.Item().Text($"ID: {id}").AlignCenter().FontColor(Colors.Green.Medium);
                        column.Item().Text($"Valid To:  { DateTime.Now.AddYears(1).ToShortDateString()}").AlignCenter().FontColor(Colors.Green.Medium);
                    });

                    //page.Footer()
                    //.AlignCenter();
                    //.Text(x =>
                    //{
                    //    x.Span("Page ");
                    //    x.CurrentPageNumber();
                    //});
                });
            })
            .GeneratePdf(attachmentFile);
        }



    }
}
