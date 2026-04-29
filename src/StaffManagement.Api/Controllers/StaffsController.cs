using Microsoft.AspNetCore.Mvc;
using StaffManagement.Api.Services;
using StaffManagement.Shared.Requests;

namespace StaffManagement.Api.Controllers;

[ApiController]
[Route("api/staffs")]
public sealed class StaffsController(IStaffService staffService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Search([FromQuery] StaffSearchRequest request, CancellationToken cancellationToken)
        => Ok(await staffService.SearchAsync(request, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var staff = await staffService.GetByIdAsync(id, cancellationToken);
        return staff is null ? NotFound() : Ok(staff);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] StaffUpsertRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await staffService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] StaffUpsertRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await staffService.UpdateAsync(id, request, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await staffService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
