using MediatR;

namespace Clinical.Application.Features.Appointments.DeleteAppointment;

public sealed record DeleteAppointmentCommand(Guid Id, Guid PatientId) : IRequest;
