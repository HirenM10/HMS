using Clinical.Application.Common.Exceptions;
using Clinical.Application.Features.Appointments.CreateAppointment;
using Clinical.Domain.Entities;
using Clinical.Domain.Enums;
using Clinical.Infrastructure.Caching;
using Clinical.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace Clinical.UnitTests.Features.Appointments;

public sealed class CreateAppointmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateAppointmentForExistingPatient()
    {
        await using var dbContext = TestApplicationDbContext.Create(nameof(Handle_ShouldCreateAppointmentForExistingPatient));
        var patient = Patient.Create("Liam", "Cole", new DateOnly(1988, 7, 3), Gender.Male, "+15552223333");
        dbContext.Patients.Add(patient);
        await dbContext.SaveChangesAsync();

        var handler = new CreateAppointmentCommandHandler(
            dbContext,
            new CacheInvalidator(new MemoryQueryCache(new MemoryCache(new MemoryCacheOptions()))));

        var result = await handler.Handle(
            new CreateAppointmentCommand(patient.Id, "Dr. Nora Allen", DateTime.UtcNow.AddDays(1)),
            CancellationToken.None);

        result.PatientId.Should().Be(patient.Id);
        dbContext.Appointments.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenPatientDoesNotExist()
    {
        await using var dbContext = TestApplicationDbContext.Create(nameof(Handle_ShouldThrowWhenPatientDoesNotExist));
        var handler = new CreateAppointmentCommandHandler(
            dbContext,
            new CacheInvalidator(new MemoryQueryCache(new MemoryCache(new MemoryCacheOptions()))));

        var action = () => handler.Handle(
            new CreateAppointmentCommand(Guid.NewGuid(), "Dr. Nora Allen", DateTime.UtcNow.AddDays(1)),
            CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}
