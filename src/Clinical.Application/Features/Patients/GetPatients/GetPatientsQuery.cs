using Clinical.Application.Common.Models;
using Clinical.Application.Features.Patients.Common;
using Clinical.Domain.Enums;
using MediatR;

namespace Clinical.Application.Features.Patients.GetPatients;

public sealed record GetPatientsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    Gender? Gender = null,
    string? SortBy = null,
    bool Descending = false) : PagedQuery(PageNumber, PageSize, SortBy, Descending), IRequest<PagedResult<PatientDto>>;
