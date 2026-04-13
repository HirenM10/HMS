using FluentValidation;

namespace Clinical.Application.Features.Appointments.CreateAppointment;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AppointmentDate).GreaterThan(DateTime.UtcNow.AddMinutes(-1));
    }
}
