using Clinical.Application.Abstractions.Caching;
using Clinical.Application.Abstractions.Messaging;

namespace Clinical.Infrastructure.Caching;

public sealed class CacheInvalidator(IQueryCache queryCache) : ICacheInvalidator
{
    public void InvalidatePatients() => queryCache.RemoveByPrefix("patients:list:");

    public void InvalidatePatient(Guid patientId) => queryCache.Remove($"patients:{patientId}");

    public void InvalidateAppointments() => queryCache.RemoveByPrefix("appointments:list:");

    public void InvalidateAppointment(Guid appointmentId) => queryCache.Remove($"appointments:{appointmentId}");
}
