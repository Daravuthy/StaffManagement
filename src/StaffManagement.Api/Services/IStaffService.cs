using StaffManagement.Shared.Requests;
using StaffManagement.Shared.Responses;

namespace StaffManagement.Api.Services;

public interface IStaffService
{
    Task<IReadOnlyList<StaffDto>> SearchAsync(StaffSearchRequest request, CancellationToken cancellationToken);
    Task<StaffDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StaffDto> CreateAsync(StaffUpsertRequest request, CancellationToken cancellationToken);
    Task<StaffDto?> UpdateAsync(Guid id, StaffUpsertRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
