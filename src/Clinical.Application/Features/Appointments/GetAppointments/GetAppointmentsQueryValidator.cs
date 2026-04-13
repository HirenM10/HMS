using FluentValidation;

namespace Clinical.Application.Features.Appointments.GetAppointments;

public sealed class GetAppointmentsQueryValidator : AbstractValidator<GetAppointmentsQuery>
{
    public GetAppointmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.DoctorName).MaximumLength(200);
        RuleFor(x => x.SortBy).MaximumLength(50);
    }
}
