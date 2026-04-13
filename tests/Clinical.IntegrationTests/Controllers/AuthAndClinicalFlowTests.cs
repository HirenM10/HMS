using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Clinical.API.Authentication;
using Clinical.Application.Common.Models;
using Clinical.Application.Features.Appointments.Common;
using Clinical.Application.Features.Appointments.CreateAppointment;
using Clinical.Application.Features.Patients.Common;
using Clinical.Application.Features.Patients.CreatePatient;
using Clinical.Domain.Enums;
using Clinical.IntegrationTests.Common;
using FluentAssertions;

namespace Clinical.IntegrationTests.Controllers;

public sealed class AuthAndClinicalFlowTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string _adminPassword;

    public AuthAndClinicalFlowTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _adminPassword = factory.AdminPassword;
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnCreated()
    {
        await AuthenticateAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/v1/patients",
            new CreatePatientCommand("Mila", "Hart", new DateOnly(1992, 1, 20), Gender.Female, "+15553456789"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<PatientDto>())!.LastName.Should().Be("Hart");
    }

    [Fact]
    public async Task CreateAppointment_ShouldReturnCreated()
    {
        await AuthenticateAsync();
        var patients = await _client.GetFromJsonAsync<PagedResult<PatientDto>>("/api/v1/patients");
        var patientId = patients!.Items.First().Id;

        var response = await _client.PostAsJsonAsync(
            "/api/v1/appointments",
            new CreateAppointmentCommand(patientId, "Dr. Laura Kent", DateTime.UtcNow.AddDays(2)));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<AppointmentDto>())!.PatientId.Should().Be(patientId);
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnBadRequest_WhenValidationFails()
    {
        await AuthenticateAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/v1/patients",
            new CreatePatientCommand(string.Empty, string.Empty, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), Gender.Unknown, "1"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPatients_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        var response = await _client.GetAsync("/api/v1/patients");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAppointment_ShouldReturnNotFound_WhenPatientMissing()
    {
        await AuthenticateAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/v1/appointments",
            new CreateAppointmentCommand(Guid.NewGuid(), "Dr. Missing", DateTime.UtcNow.AddDays(1)));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task AuthenticateAsync()
    {
        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("admin@clinical.local", _adminPassword));
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await login.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
    }
}
