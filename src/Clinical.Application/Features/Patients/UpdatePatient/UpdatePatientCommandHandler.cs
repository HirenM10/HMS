using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Application.Common.Exceptions;
using Clinical.Application.Features.Patients.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Patients.UpdatePatient;

public sealed class UpdatePatientCommandHandler(
    IApplicationDbContext dbContext,
    ICacheInvalidator cacheInvalidator) : IRequestHandler<UpdatePatientCommand, PatientDto>
{
    public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .Include(x => x.Appointments)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Patient '{request.Id}' was not found.");

        patient.UpdateDetails(request.FirstName, request.LastName, request.DateOfBirth, request.Gender, request.ContactNumber);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheInvalidator.InvalidatePatient(request.Id);
        cacheInvalidator.InvalidatePatients();
        return patient.ToDto();
    }
}
