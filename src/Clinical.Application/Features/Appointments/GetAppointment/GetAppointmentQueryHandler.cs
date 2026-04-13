using Clinical.Application.Abstractions.Caching;
using Clinical.Application.Abstractions.Data;
using Clinical.Application.Common.Exceptions;
using Clinical.Application.Features.Appointments.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Appointments.GetAppointment;

public sealed class GetAppointmentQueryHandler(
    IApplicationDbContext dbContext,
    IQueryCache cache) : IRequestHandler<GetAppointmentQuery, AppointmentDto>
{
    public Task<AppointmentDto> Handle(GetAppointmentQuery request, CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"appointments:{request.Id}",
            async ct =>
            {
                var appointment = await dbContext.Appointments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.Id, ct);
                return appointment?.ToDto() ?? throw new NotFoundException($"Appointment '{request.Id}' was not found.");
            },
            TimeSpan.FromMinutes(5),
            cancellationToken);
}
