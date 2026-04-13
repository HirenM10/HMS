using Clinical.Application.Common.Models;
using Clinical.Application.Features.Appointments.Common;
using Clinical.Domain.Enums;
using MediatR;

namespace Clinical.Application.Features.Appointments.GetAppointments;

public sealed record GetAppointmentsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? PatientId = null,
    AppointmentStatus? Status = null,
    string? DoctorName = null,
    string? SortBy = null,
    bool Descending = false) : PagedQuery(PageNumber, PageSize, SortBy, Descending), IRequest<PagedResult<AppointmentDto>>;
