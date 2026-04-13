using Clinical.Domain.Enums;

namespace Clinical.Application.Features.Appointments.Common;

public sealed record AppointmentDto(
    Guid Id,
    Guid PatientId,
    string DoctorName,
    DateTime AppointmentDate,
    AppointmentStatus Status);
