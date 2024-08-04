using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace access_control.core.Queries.Lock
{
    public class HandleFetchAssignedLocks
    {
        public class Query : IRequest<GenericResponse<List<Result>>>
        {
            public string RoleId { get; set; }
            public string TenantId { get; set; }
        }

        public class Result
        {
            public string Id { get; set; }
            public string Name { get; set; }
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
                var locks = await (from p in _dbContext.Permissions
                                   .Where(e => e.TenantId == request.TenantId && e.RoleId == request.RoleId)
                                   join l in _dbContext.Locks on p.LockId equals l.Id
                                   select new Result
                                   {
                                       Id = l.Id,
                                       Name = l.Name
                                   }).ToListAsync();

                if (!locks.Any())
                    return GenericResponse<List<Result>>.Fail("No locks assigned yet...");
               
                _logger.LogError($"Retrieved assigned locks for {request.RoleId} successfully at {DateTime.Now}");
                return GenericResponse<List<Result>>.Success(locks, "Locks retrieved successfully.");
            }
        }
    }
}