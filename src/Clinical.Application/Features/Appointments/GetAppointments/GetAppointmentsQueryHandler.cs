using Clinical.Application.Abstractions.Caching;
using Clinical.Application.Abstractions.Data;
using Clinical.Application.Common.Models;
using Clinical.Application.Features.Appointments.Common;
using Clinical.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Appointments.GetAppointments;

public sealed class GetAppointmentsQueryHandler(
    IApplicationDbContext dbContext,
    IQueryCache cache) : IRequestHandler<GetAppointmentsQuery, PagedResult<AppointmentDto>>
{
    public Task<PagedResult<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"appointments:list:{request.PageNumber}:{request.PageSize}:{request.PatientId}:{request.Status}:{request.DoctorName}:{request.SortBy}:{request.Descending}";

        return cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                IQueryable<Appointment> query = dbContext.Appointments.AsNoTracking();

                if (request.PatientId.HasValue)
                {
                    query = query.Where(x => x.PatientId == request.PatientId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(x => x.Status == request.Status.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.DoctorName))
                {
                    var doctorName = request.DoctorName.Trim();
                    query = query.Where(x => x.DoctorName.Contains(doctorName));
                }

                query = (request.SortBy?.ToLowerInvariant(), request.Descending) switch
                {
                    ("doctorname", false) => query.OrderBy(x => x.DoctorName),
                    ("doctorname", true) => query.OrderByDescending(x => x.DoctorName),
                    ("status", false) => query.OrderBy(x => x.Status),
                    ("status", true) => query.OrderByDescending(x => x.Status),
                    ("appointmentdate", false) => query.OrderBy(x => x.AppointmentDate),
                    ("appointmentdate", true) => query.OrderByDescending(x => x.AppointmentDate),
                    _ when request.Descending => query.OrderByDescending(x => x.AppointmentDate),
                    _ => query.OrderBy(x => x.AppointmentDate)
                };

                var totalCount = await query.CountAsync(ct);
                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(ct);

                return new PagedResult<AppointmentDto>
                {
                    Items = items.Select(x => x.ToDto()).ToArray(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };
            },
            TimeSpan.FromMinutes(2),
            cancellationToken);
    }
}
