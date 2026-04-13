using Clinical.Domain.Common;

namespace Clinical.Domain.Events;

public sealed record AppointmentCreatedDomainEvent(
    Guid PatientId,
    Guid AppointmentId,
    DateTime AppointmentDate,
    string DoctorName) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
