using FluentValidation;

namespace Clinical.Application.Features.Appointments.UpdateAppointment;

public sealed class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AppointmentDate).GreaterThan(DateTime.UtcNow.AddYears(-5));
        RuleFor(x => x.Status).IsInEnum();
    }
}
