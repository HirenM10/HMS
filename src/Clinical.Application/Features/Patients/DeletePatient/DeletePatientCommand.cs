using MediatR;

namespace Clinical.Application.Features.Patients.DeletePatient;

public sealed record DeletePatientCommand(Guid Id) : IRequest;
