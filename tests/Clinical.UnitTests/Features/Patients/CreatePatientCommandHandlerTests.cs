using Clinical.Application.Features.Patients.CreatePatient;
using Clinical.Domain.Enums;
using Clinical.Infrastructure.Caching;
using Clinical.UnitTests.Common;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace Clinical.UnitTests.Features.Patients;

public sealed class CreatePatientCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreatePatient()
    {
        await using var dbContext = TestApplicationDbContext.Create(nameof(Handle_ShouldCreatePatient));
        var handler = new CreatePatientCommandHandler(
            dbContext,
            new CacheInvalidator(new MemoryQueryCache(new MemoryCache(new MemoryCacheOptions()))));

        var result = await handler.Handle(
            new CreatePatientCommand("Ava", "Stone", new DateOnly(1995, 6, 12), Gender.Female, "+15550001111"),
            CancellationToken.None);

        result.FirstName.Should().Be("Ava");
        dbContext.Patients.Should().ContainSingle();
    }

    [Fact]
    public async Task Validator_ShouldRejectInvalidRequest()
    {
        var validator = new CreatePatientCommandValidator();

        var result = await validator.ValidateAsync(
            new CreatePatientCommand(string.Empty, string.Empty, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), Gender.Unknown, "1"));

        result.IsValid.Should().BeFalse();
    }
}
