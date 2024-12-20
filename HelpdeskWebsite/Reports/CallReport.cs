using HelpdeskDAL;
using HelpdeskViewModels;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
namespace HelpdeskWebsite.Reports
{
    public class CallReport
    {
        public async Task GenerateReport(string rootpath)
        {
            PageSize pg = PageSize.A4;
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/callreport.pdf",
            new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf); // PageSize(595, 842)
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Calls")
            .SetFont(helvetica)
           .SetFontSize(24)
            .SetBold()
           .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/istockphoto-1412305573-612x612.jpg"))
            .ScaleAbsolute(200, 100)
            .SetFixedPosition(((pg.GetWidth() - 200) / 2), 750));
            Table table = new(6);
            table.AddCell(new Cell().Add(new Paragraph("Opened")
            .SetFontSize(14)
            .SetBold()
            .SetPaddingLeft(10)
            .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Call Name")
            .SetFontSize(14)
            .SetBold()
            .SetPaddingLeft(20)
            .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Tech Person")
            .SetFontSize(14)
           .SetBold()
            .SetPaddingLeft(30)
           .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Problem")
            .SetFontSize(14)
            .SetBold()
            .SetPaddingLeft(40)
            .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Status")
            .SetFontSize(14)
            .SetBold()
            .SetPaddingLeft(50)
            .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Closed")
            .SetBold()
           .SetFontSize(14)
            .SetPaddingLeft(60)
            .SetTextAlignment(TextAlignment.CENTER))
           .SetBorder(Border.NO_BORDER));

            // Create an instance of CallViewModel
            CallViewModel viewModel = new();

            // Retrieve all calls from the database
            List<CallViewModel> calls = await viewModel.GetAll();

            // Print the list of calls
            foreach (CallViewModel call in calls)
            {
                table.AddCell(new Cell().Add(new Paragraph(call.DateOpened.ToShortDateString())
                .SetFontSize(10)
               .SetPaddingLeft(10)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.EmployeeName)
                .SetFontSize(10)
               .SetPaddingLeft(20)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.TechName)
                .SetFontSize(10)
               .SetPaddingLeft(30)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.ProblemDescription)
                .SetFontSize(10)
               .SetPaddingLeft(40)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.OpenStatus.ToString() == "True" ? "Opened": "Closed")
                .SetFontSize(10)
               .SetPaddingLeft(50)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.DateClosed == null ? "--" : call.DateClosed.Value.ToShortDateString())
                .SetFontSize(10)
               .SetPaddingLeft(60)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
            }


            document.Add(table);
            document.Add(new Paragraph("Call report written on - " + DateTime.Now)
            .SetFontSize(6)
            .SetTextAlignment(TextAlignment.CENTER));
            document.Close();



        }

    }
}
