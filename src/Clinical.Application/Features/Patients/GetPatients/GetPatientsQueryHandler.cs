using Clinical.Application.Abstractions.Caching;
using Clinical.Application.Abstractions.Data;
using Clinical.Application.Common.Models;
using Clinical.Application.Features.Patients.Common;
using Clinical.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Patients.GetPatients;

public sealed class GetPatientsQueryHandler(
    IApplicationDbContext dbContext,
    IQueryCache cache) : IRequestHandler<GetPatientsQuery, PagedResult<PatientDto>>
{
    public Task<PagedResult<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"patients:list:{request.PageNumber}:{request.PageSize}:{request.Search}:{request.Gender}:{request.SortBy}:{request.Descending}";

        return cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                IQueryable<Patient> query = dbContext.Patients.Include(x => x.Appointments).AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim();
                    query = query.Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search));
                }

                if (request.Gender.HasValue)
                {
                    query = query.Where(x => x.Gender == request.Gender.Value);
                }

                query = (request.SortBy?.ToLowerInvariant(), request.Descending) switch
                {
                    ("firstname", false) => query.OrderBy(x => x.FirstName),
                    ("firstname", true) => query.OrderByDescending(x => x.FirstName),
                    ("lastname", false) => query.OrderBy(x => x.LastName),
                    ("lastname", true) => query.OrderByDescending(x => x.LastName),
                    ("dateofbirth", false) => query.OrderBy(x => x.DateOfBirth),
                    ("dateofbirth", true) => query.OrderByDescending(x => x.DateOfBirth),
                    _ when request.Descending => query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName),
                    _ => query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                };

                var totalCount = await query.CountAsync(ct);
                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(ct);

                return new PagedResult<PatientDto>
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
