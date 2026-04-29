using StaffManagement.Shared.Enums;

namespace StaffManagement.Shared.Responses;

public sealed class StaffDto
{
    public Guid Id { get; set; }
    public string StaffId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly Birthday { get; set; }
    public Gender Gender { get; set; }
}
