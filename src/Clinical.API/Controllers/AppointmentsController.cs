using Asp.Versioning;
using Clinical.Application.Common.Models;
using Clinical.Application.Features.Appointments.Common;
using Clinical.Application.Features.Appointments.CreateAppointment;
using Clinical.Application.Features.Appointments.DeleteAppointment;
using Clinical.Application.Features.Appointments.GetAppointment;
using Clinical.Application.Features.Appointments.GetAppointments;
using Clinical.Application.Features.Appointments.UpdateAppointment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/appointments")]
[Authorize(Policy = "RequireClinicalRead")]
public sealed class AppointmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<AppointmentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AppointmentDto>>> GetAppointments([FromQuery] GetAppointmentsQuery query, CancellationToken cancellationToken)
        => Ok(await mediator.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AppointmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetAppointmentQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Policy = "RequireClinicalWrite")]
    [ProducesResponseType<AppointmentDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAppointment), new { id = result.Id, version = "1" }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireClinicalWrite")]
    [ProducesResponseType<AppointmentDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command with { Id = id }, cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAppointment(Guid id, [FromQuery] Guid patientId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAppointmentCommand(id, patientId), cancellationToken);
        return NoContent();
    }
}
