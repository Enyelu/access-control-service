using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace access_control.core.Queries.Lock
{
    public class HandleFetchAllocatedLocks
    {
        public class Query : IRequest<GenericResponse<List<Result>>>
        {
           public string TenantId { get; set; }
        }

        public class Result
        {
            public string Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Name { get; set; }
            public bool IsOpen { get; set; }
            public string? TenantId { get; set; }
            public string SerialNumber { get; set; }
        }
        public class Handler : IRequestHandler<Query, GenericResponse<List<Result>>>
        {
            public readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly ApplicationContext _dbContext;

            public Handler(ApplicationContext dbContext, ILogger<Handler> logger, IMapper mapper)
            {
                _dbContext = dbContext;
                _logger = logger;
                _dbContext = dbContext;
                _mapper = mapper;
            }
            public async Task<GenericResponse<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
            {
                _logger.LogError($"Attempting to retrieve allocated locks for {request.TenantId} at {DateTime.Now}");

                var locks = await _dbContext.Locks.Where(x => x.TenantId == request.TenantId)
                    .ToListAsync(cancellationToken);

                if(!locks.Any())
                    return GenericResponse<List<Result>>.Fail("No locks allocated yet...");

                var orderedLocks = locks.OrderByDescending(x => x.CreatedAt);
                var mappedLocks = _mapper.Map<List<Result>>(orderedLocks);
                _logger.LogError($"Retrieved allocated locks for {request.TenantId} successfully at {DateTime.Now}");
                return GenericResponse<List<Result>>.Success(mappedLocks, "Locks retrieved successfully.");
            }
        }
    }
}
