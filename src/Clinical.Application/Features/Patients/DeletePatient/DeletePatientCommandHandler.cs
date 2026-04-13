using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Patients.DeletePatient;

public sealed class DeletePatientCommandHandler(
    IApplicationDbContext dbContext,
    ICacheInvalidator cacheInvalidator) : IRequestHandler<DeletePatientCommand>
{
    public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Patient '{request.Id}' was not found.");

        dbContext.Patients.Remove(patient);
        await dbContext.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidatePatient(request.Id);
        cacheInvalidator.InvalidatePatients();
        cacheInvalidator.InvalidateAppointments();
    }
}
