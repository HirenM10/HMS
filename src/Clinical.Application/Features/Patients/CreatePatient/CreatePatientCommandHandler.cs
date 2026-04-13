using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Application.Features.Patients.Common;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Application.Features.Patients.CreatePatient;

public sealed class CreatePatientCommandHandler(
    IApplicationDbContext dbContext,
    ICacheInvalidator cacheInvalidator) : IRequestHandler<CreatePatientCommand, PatientDto>
{
    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = Patient.Create(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.ContactNumber);

        await dbContext.Patients.AddAsync(patient, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidatePatients();
        return patient.ToDto();
    }
}
