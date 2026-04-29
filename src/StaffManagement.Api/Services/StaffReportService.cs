using ClosedXML.Excel;
using System.Text;
using StaffManagement.Shared.Requests;

namespace StaffManagement.Api.Services;

public sealed class StaffReportService(IStaffService staffService) : IStaffReportService
{
    public async Task<byte[]> ExportExcelAsync(StaffSearchRequest request, CancellationToken cancellationToken)
    {
        var staffs = await staffService.SearchAsync(request, cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Staffs");

        worksheet.Cell(1, 1).Value = "Staff ID";
        worksheet.Cell(1, 2).Value = "Full Name";
        worksheet.Cell(1, 3).Value = "Birthday";
        worksheet.Cell(1, 4).Value = "Gender";
        worksheet.Row(1).Style.Font.Bold = true;

        for (var index = 0; index < staffs.Count; index++)
        {
            var row = index + 2;
            worksheet.Cell(row, 1).Value = staffs[index].StaffId;
            worksheet.Cell(row, 2).Value = staffs[index].FullName;
            worksheet.Cell(row, 3).Value = staffs[index].Birthday.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 4).Value = staffs[index].Gender.ToString();
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportPdfAsync(StaffSearchRequest request, CancellationToken cancellationToken)
    {
        var staffs = await staffService.SearchAsync(request, cancellationToken);
        return SimplePdfBuilder.Build(staffs.Select(staff =>
            $"{staff.StaffId} | {staff.FullName} | {staff.Birthday:yyyy-MM-dd} | {staff.Gender}"));
    }

    private static class SimplePdfBuilder
    {
        public static byte[] Build(IEnumerable<string> lines)
        {
            var content = BuildContent(lines);
            var objects = new[]
            {
                "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
                "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj",
                "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 5 0 R /Resources << /Font << /F1 4 0 R >> >> >> endobj",
                "4 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj",
                $"5 0 obj << /Length {Encoding.ASCII.GetByteCount(content)} >> stream\n{content}\nendstream endobj"
            };

            var builder = new StringBuilder();
            builder.AppendLine("%PDF-1.4");

            var offsets = new List<int>();
            foreach (var obj in objects)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(builder.ToString()));
                builder.AppendLine(obj);
            }

            var xrefPosition = Encoding.ASCII.GetByteCount(builder.ToString());
            builder.AppendLine("xref");
            builder.AppendLine($"0 {objects.Length + 1}");
            builder.AppendLine("0000000000 65535 f ");

            foreach (var offset in offsets)
            {
                builder.AppendLine($"{offset:D10} 00000 n ");
            }

            builder.AppendLine("trailer");
            builder.AppendLine($"<< /Size {objects.Length + 1} /Root 1 0 R >>");
            builder.AppendLine("startxref");
            builder.AppendLine(xrefPosition.ToString());
            builder.Append("%%EOF");

            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        private static string BuildContent(IEnumerable<string> lines)
        {
            var content = new StringBuilder();
            content.AppendLine("BT");
            content.AppendLine("/F1 18 Tf");
            content.AppendLine("50 790 Td");
            content.AppendLine("(Staff Search Report) Tj");
            content.AppendLine("0 -24 Td");
            content.AppendLine("/F1 11 Tf");

            foreach (var line in lines.DefaultIfEmpty("No staff records found."))
            {
                content.AppendLine($"({Escape(line)}) Tj");
                content.AppendLine("0 -16 Td");
            }

            content.Append("ET");
            return content.ToString();
        }

        private static string Escape(string value) =>
            value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    }
}
