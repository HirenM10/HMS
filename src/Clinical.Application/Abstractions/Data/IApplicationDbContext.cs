using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Appointment> Appointments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
