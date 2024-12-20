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
    public class EmployeeReport
    {
        public async Task GenerateReport(string rootpath)
        {
            PageSize pg = PageSize.A4;
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/employeereport.pdf",
            new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf); // PageSize(595, 842)
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Employees")
            .SetFont(helvetica)
           .SetFontSize(24)
            .SetBold()
           .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/istockphoto-1412305573-612x612.jpg"))
            .ScaleAbsolute(200, 100)
            .SetFixedPosition(((pg.GetWidth() - 200) / 2), 750));
            Table table = new(3);
            table.AddCell(new Cell().Add(new Paragraph("Title")
            .SetFontSize(16)
            .SetBold()
            .SetPaddingLeft(120)
            .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("First Name")
            .SetFontSize(16)
           .SetBold()
            .SetPaddingLeft(45)
           .SetTextAlignment(TextAlignment.CENTER))
            .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
            .SetBold()
           .SetFontSize(16)
            .SetPaddingLeft(30)
            .SetTextAlignment(TextAlignment.CENTER))
           .SetBorder(Border.NO_BORDER));

            // Create an instance of EmployeeViewModel
            EmployeeViewModel viewModel = new();

            // Retrieve all employees from the database
            List<EmployeeViewModel> employees = await viewModel.GetAll();

            // Print the list of employees
            foreach (var employee in employees)
            {
                table.AddCell(new Cell().Add(new Paragraph(employee.Title)
                .SetFontSize(14)
               .SetPaddingLeft(120)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.Firstname)
                .SetFontSize(14)
               .SetPaddingLeft(45)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.Lastname)
                .SetFontSize(14)
               .SetPaddingLeft(30)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
            }


            document.Add(table);
            document.Add(new Paragraph("Employee report written on - " + DateTime.Now)
            .SetFontSize(6)
            .SetTextAlignment(TextAlignment.CENTER));
            document.Close();



        }

    }
}

public class Employee
{
    public string? Title { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
}