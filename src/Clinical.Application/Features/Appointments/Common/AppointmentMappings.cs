using Clinical.Domain.Entities;

namespace Clinical.Application.Features.Appointments.Common;

public static class AppointmentMappings
{
    public static AppointmentDto ToDto(this Appointment appointment) =>
        new(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorName,
            appointment.AppointmentDate,
            appointment.Status);
}
