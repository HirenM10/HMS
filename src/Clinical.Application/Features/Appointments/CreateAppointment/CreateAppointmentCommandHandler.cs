using Clinical.Application.Abstractions.Data;
using Clinical.Application.Abstractions.Messaging;
using Clinical.Application.Common.Exceptions;
using Clinical.Application.Features.Appointments.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Appointments.CreateAppointment;

public sealed class CreateAppointmentCommandHandler(
    IApplicationDbContext dbContext,
    ICacheInvalidator cacheInvalidator) : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients
            .Include(x => x.Appointments)
            .SingleOrDefaultAsync(x => x.Id == request.PatientId, cancellationToken)
            ?? throw new NotFoundException($"Patient '{request.PatientId}' was not found.");

        var appointment = patient.ScheduleAppointment(request.DoctorName, request.AppointmentDate);
        await dbContext.Appointments.AddAsync(appointment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        cacheInvalidator.InvalidatePatient(request.PatientId);
        cacheInvalidator.InvalidatePatients();
        cacheInvalidator.InvalidateAppointments();
        return appointment.ToDto();
    }
}
