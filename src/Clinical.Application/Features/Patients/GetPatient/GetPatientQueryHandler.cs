using Clinical.Application.Abstractions.Caching;
using Clinical.Application.Abstractions.Data;
using Clinical.Application.Common.Exceptions;
using Clinical.Application.Features.Patients.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Application.Features.Patients.GetPatient;

public sealed class GetPatientQueryHandler(
    IApplicationDbContext dbContext,
    IQueryCache cache) : IRequestHandler<GetPatientQuery, PatientDto>
{
    public Task<PatientDto> Handle(GetPatientQuery request, CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            $"patients:{request.Id}",
            async ct =>
            {
                var patient = await dbContext.Patients
                    .Include(x => x.Appointments)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == request.Id, ct);

                return patient?.ToDto() ?? throw new NotFoundException($"Patient '{request.Id}' was not found.");
            },
            TimeSpan.FromMinutes(5),
            cancellationToken);
}
