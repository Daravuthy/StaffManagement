using System.ComponentModel.DataAnnotations;
using StaffManagement.Shared.Enums;

namespace StaffManagement.Shared.Requests;

public sealed class StaffUpsertRequest
{
    [Required]
    [StringLength(8, MinimumLength = 8)]
    public string StaffId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateOnly? Birthday { get; set; }

    [Required]
    [Range(1, 2)]
    public Gender? Gender { get; set; }
}
