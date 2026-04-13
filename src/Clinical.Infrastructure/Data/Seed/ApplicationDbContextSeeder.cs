using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Infrastructure.Data.Seed;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Patients.AnyAsync(cancellationToken))
        {
            return;
        }

        var patient1 = Patient.Create("Jane", "Doe", new DateOnly(1990, 8, 12), Gender.Female, "+15551234567");
        patient1.ScheduleAppointment("Dr. Avery Stone", DateTime.UtcNow.AddDays(2));

        var patient2 = Patient.Create("John", "Smith", new DateOnly(1984, 3, 5), Gender.Male, "+15557654321");
        patient2.ScheduleAppointment("Dr. Maya Reed", DateTime.UtcNow.AddDays(5));

        await dbContext.Patients.AddRangeAsync([patient1, patient2], cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
