using StaffManagement.Shared.Enums;

namespace StaffManagement.Shared.Requests;

public sealed class StaffSearchRequest
{
    public string? StaffId { get; set; }
    public string? FullName { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? BirthdayFrom { get; set; }
    public DateOnly? BirthdayTo { get; set; }
}
