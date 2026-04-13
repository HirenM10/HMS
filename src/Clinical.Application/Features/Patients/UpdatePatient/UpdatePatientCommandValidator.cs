using Clinical.Domain.Enums;
using FluentValidation;

namespace Clinical.Application.Features.Patients.UpdatePatient;

public sealed class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
        RuleFor(x => x.Gender).IsInEnum().NotEqual(Gender.Unknown);
        RuleFor(x => x.ContactNumber).NotEmpty().MaximumLength(20);
    }
}
