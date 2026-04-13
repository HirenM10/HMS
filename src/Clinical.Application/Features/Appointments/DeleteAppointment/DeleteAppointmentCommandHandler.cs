using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Appointments.DeleteAppointment;

public sealed class DeleteAppointmentCommandHandler(
    IApplicationDbContext dbContext,
    ICacheInvalidator cacheInvalidator) : IRequestHandler<DeleteAppointmentCommand>
{
    public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .Include(x => x.Appointments)
            .SingleOrDefaultAsync(x => x.Id == request.PatientId, cancellationToken)
            ?? throw new NotFoundException($"Patient '{request.PatientId}' was not found.");

        patient.RemoveAppointment(request.Id);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheInvalidator.InvalidatePatient(request.PatientId);
        cacheInvalidator.InvalidatePatients();
        cacheInvalidator.InvalidateAppointment(request.Id);
        cacheInvalidator.InvalidateAppointments();
    }
}
