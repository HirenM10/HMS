using Clinical.Domain.Enums;

namespace Clinical.Application.Features.Patients.Common;

public sealed record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    string ContactNumber,
    int AppointmentCount);
