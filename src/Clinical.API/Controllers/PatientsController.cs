using Asp.Versioning;
using Clinical.Application.Common.Models;
using Clinical.Application.Features.Patients.Common;
using Clinical.Application.Features.Patients.CreatePatient;
using Clinical.Application.Features.Patients.DeletePatient;
using Clinical.Application.Features.Patients.GetPatient;
using Clinical.Application.Features.Patients.GetPatients;
using Clinical.Application.Features.Patients.UpdatePatient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/patients")]
[Authorize(Policy = "RequireClinicalRead")]
public sealed class PatientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<PatientDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PatientDto>>> GetPatients([FromQuery] GetPatientsQuery query, CancellationToken cancellationToken)
        => Ok(await mediator.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PatientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetPatientQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Policy = "RequireClinicalWrite")]
    [ProducesResponseType<PatientDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPatient), new { id = result.Id, version = "1" }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequireClinicalWrite")]
    [ProducesResponseType<PatientDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PatientDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command with { Id = id }, cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePatient(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeletePatientCommand(id), cancellationToken);
        return NoContent();
    }
}
