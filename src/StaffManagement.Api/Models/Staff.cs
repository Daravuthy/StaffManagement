using StaffManagement.Shared.Enums;

namespace StaffManagement.Api.Models;

public sealed class Staff
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StaffId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly Birthday { get; set; }
    public Gender Gender { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
