using Clinical.Domain.Entities;

namespace Clinical.Application.Features.Patients.Common;

public static class PatientMappings
{
    public static PatientDto ToDto(this Patient patient) =>
        new(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.DateOfBirth,
            patient.Gender,
            patient.ContactNumber.Value,
            patient.Appointments.Count);
}
