using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace access_control.core.Queries.Permission
{
    public class HandlePermissionByTenantId
    {
        public class Query : IRequest<GenericResponse<PaginationResult<Result>>>
        {
            public Guid TenantId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int PageSize { get; set; }
            public int PageNumber { get; set; }
        }

        public class Result
        {
            public string Id { get; set; }
            public string RoleId { get; set; }
            public string LockId { get; set; }
            public string TenantId { get; set; }

        }
        public class Handler : IRequestHandler<Query, GenericResponse<PaginationResult<Result>>>
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
            public async Task<GenericResponse<PaginationResult<Result>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.End <= DateTime.MinValue)
                    request.End = DateTime.Now;

                if (request.Start <= DateTime.MinValue)
                    request.Start = request.End.AddMonths(-6);

                var tenantId = request.TenantId.ToString();

                var permissions = await _dbContext.Permissions.Where(x => 
                x.TenantId == tenantId &&
                x.CreatedAt >= request.Start &&
                x.CreatedAt <= request.End).ToListAsync(cancellationToken);

                if (!permissions.Any())
                    GenericResponse<string>.Fail($"No permission(s) found between {request.Start} and {request.End}");

                var result = _mapper.Map<List<Result>>(permissions);
                var pagedResult = Paginator<Result>.GetPagedData(result, request.PageNumber, request.PageSize);
                return GenericResponse<PaginationResult<Result>>.Success(pagedResult, "successful");
            }
        }
    }
}
