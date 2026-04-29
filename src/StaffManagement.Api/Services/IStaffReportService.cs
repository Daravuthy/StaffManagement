using StaffManagement.Shared.Requests;

namespace StaffManagement.Api.Services;

public interface IStaffReportService
{
    Task<byte[]> ExportExcelAsync(StaffSearchRequest request, CancellationToken cancellationToken);
    Task<byte[]> ExportPdfAsync(StaffSearchRequest request, CancellationToken cancellationToken);
}
