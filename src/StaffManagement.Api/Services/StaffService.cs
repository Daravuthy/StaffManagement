using Microsoft.EntityFrameworkCore;
using StaffManagement.Api.Data;
using StaffManagement.Api.Models;
using StaffManagement.Shared.Requests;
using StaffManagement.Shared.Responses;

namespace StaffManagement.Api.Services;

public sealed class StaffService(AppDbContext dbContext) : IStaffService
{
    public async Task<IReadOnlyList<StaffDto>> SearchAsync(StaffSearchRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Staffs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.StaffId))
        {
            var staffId = request.StaffId.Trim();
            query = query.Where(x => x.StaffId.Contains(staffId));
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            var fullName = request.FullName.Trim();
            query = query.Where(x => x.FullName.Contains(fullName));
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(x => x.Gender == request.Gender.Value);
        }

        if (request.BirthdayFrom.HasValue)
        {
            query = query.Where(x => x.Birthday >= request.BirthdayFrom.Value);
        }

        if (request.BirthdayTo.HasValue)
        {
            query = query.Where(x => x.Birthday <= request.BirthdayTo.Value);
        }

        return await query
            .OrderBy(x => x.StaffId)
            .Select(x => ToDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<StaffDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var staff = await dbContext.Staffs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return staff is null ? null : ToDto(staff);
    }

    public async Task<StaffDto> CreateAsync(StaffUpsertRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request, null, cancellationToken);

        var staff = new Staff
        {
            StaffId = request.StaffId.Trim(),
            FullName = request.FullName.Trim(),
            Birthday = request.Birthday!.Value,
            Gender = request.Gender!.Value
        };

        dbContext.Staffs.Add(staff);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(staff);
    }

    public async Task<StaffDto?> UpdateAsync(Guid id, StaffUpsertRequest request, CancellationToken cancellationToken)
    {
        var staff = await dbContext.Staffs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (staff is null)
        {
            return null;
        }

        await ValidateAsync(request, id, cancellationToken);

        staff.StaffId = request.StaffId.Trim();
        staff.FullName = request.FullName.Trim();
        staff.Birthday = request.Birthday!.Value;
        staff.Gender = request.Gender!.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(staff);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var staff = await dbContext.Staffs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (staff is null)
        {
            return false;
        }

        dbContext.Staffs.Remove(staff);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ValidateAsync(StaffUpsertRequest request, Guid? currentId, CancellationToken cancellationToken)
    {
        if (request.Birthday is null || request.Gender is null)
        {
            throw new ArgumentException("Birthday and gender are required.");
        }

        if (request.Birthday > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ArgumentException("Birthday cannot be in the future.");
        }

        var normalizedStaffId = request.StaffId.Trim();
        var existing = await dbContext.Staffs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StaffId == normalizedStaffId && x.Id != currentId, cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException($"Staff ID '{normalizedStaffId}' already exists.");
        }
    }

    private static StaffDto ToDto(Staff staff) =>
        new()
        {
            Id = staff.Id,
            StaffId = staff.StaffId,
            FullName = staff.FullName,
            Birthday = staff.Birthday,
            Gender = staff.Gender
        };
}
