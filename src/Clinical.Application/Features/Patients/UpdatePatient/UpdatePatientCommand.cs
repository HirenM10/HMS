using Clinical.Application.Features.Patients.Common;
using Clinical.Domain.Enums;
using MediatR;

namespace Clinical.Application.Features.Patients.UpdatePatient;

public sealed record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    string ContactNumber) : IRequest<PatientDto>;
