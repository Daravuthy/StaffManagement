using Microsoft.AspNetCore.Mvc;
using StaffManagement.Api.Services;
using StaffManagement.Shared.Requests;

namespace StaffManagement.Api.Controllers;

[ApiController]
[Route("api/reports/staffs")]
public sealed class ReportsController(IStaffReportService reportService) : ControllerBase
{
    [HttpGet("excel")]
    public async Task<FileContentResult> ExportExcel([FromQuery] StaffSearchRequest request, CancellationToken cancellationToken)
    {
        var file = await reportService.ExportExcelAsync(request, cancellationToken);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"staff-report-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet("pdf")]
    public async Task<FileContentResult> ExportPdf([FromQuery] StaffSearchRequest request, CancellationToken cancellationToken)
    {
        var file = await reportService.ExportPdfAsync(request, cancellationToken);
        return File(file, "application/pdf", $"staff-report-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }
}
