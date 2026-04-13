using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Application.Common.Exceptions;
using Clinical.Application.Features.Appointments.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Appointments.UpdateAppointment;

public sealed class UpdateAppointmentCommandHandler(
    IApplicationDbContext dbContext,
    ICacheInvalidator cacheInvalidator) : IRequestHandler<UpdateAppointmentCommand, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .Include(x => x.Appointments)
            .SingleOrDefaultAsync(x => x.Id == request.PatientId, cancellationToken)
            ?? throw new NotFoundException($"Patient '{request.PatientId}' was not found.");

        patient.UpdateAppointment(request.Id, request.DoctorName, request.AppointmentDate, request.Status);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheInvalidator.InvalidatePatient(request.PatientId);
        cacheInvalidator.InvalidatePatients();
        cacheInvalidator.InvalidateAppointment(request.Id);
        cacheInvalidator.InvalidateAppointments();

        return patient.Appointments.Single(x => x.Id == request.Id).ToDto();
    }
}
