using Clinical.Application.Features.Appointments.Common;
using MediatR;

namespace Clinical.Application.Features.Appointments.GetAppointment;

public sealed record GetAppointmentQuery(Guid Id) : IRequest<AppointmentDto>;
