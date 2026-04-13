using Clinical.Application.Features.Patients.Common;
using MediatR;

namespace Clinical.Application.Features.Patients.GetPatient;

public sealed record GetPatientQuery(Guid Id) : IRequest<PatientDto>;
