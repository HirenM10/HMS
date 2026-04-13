using Clinical.Application.Features.Appointments.Common;
using MediatR;

namespace Clinical.Application.Features.Appointments.CreateAppointment;

public sealed record CreateAppointmentCommand(
    Guid PatientId,
    string DoctorName,
    DateTime AppointmentDate) : IRequest<AppointmentDto>;
