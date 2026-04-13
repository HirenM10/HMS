using Clinical.Application.Features.Appointments.Common;
using Clinical.Domain.Enums;
using MediatR;

namespace Clinical.Application.Features.Appointments.UpdateAppointment;

public sealed record UpdateAppointmentCommand(
    Guid Id,
    Guid PatientId,
    string DoctorName,
    DateTime AppointmentDate,
    AppointmentStatus Status) : IRequest<AppointmentDto>;
