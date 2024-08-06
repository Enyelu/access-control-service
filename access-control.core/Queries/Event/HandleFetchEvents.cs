using access_control.core.Shared;
using access_control.domain.Enums;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace access_control.core.Queries.Event
{
    public class HandleFetchEvents
    {
        public class Query : IRequest<GenericResponse<PaginationResult<Result>>>
        {
            public EventEnum EventType { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int PageSize { get; set; }
            public int PageNumber { get; set; }
            public string TenantId { get; set; }
        }

        public class Result
        {
            public string Id { get; set; }
            public string Action { get; set; }
            public string Changes { get; set; }
            public bool IsSuccessful { get; set; }
            public string? CreatedBy { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ModifiedAt { get; set; }
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

                var eventsquery = _dbContext.EventLogs.Where(x =>
                x.TenantId == request.TenantId &&
                x.CreatedAt >= request.Start &&
                x.CreatedAt <= request.End);

                if(request.EventType != EventEnum.Generic)
                    eventsquery = eventsquery.Where(x => x.Action == request.EventType.ToString());
                
                var events = await eventsquery.OrderByDescending(x => x.CreatedAt).ToListAsync();

                if (!events.Any())
                    GenericResponse<string>.Fail($"No event(s) found between {request.Start} and {request.End}");

                var result = _mapper.Map<List<Result>>(events);
                var pagedResult = Paginator<Result>.GetPagedData(result, request.PageNumber, request.PageSize);
                return GenericResponse<PaginationResult<Result>>.Success(pagedResult, "successful");
            }
        }
    }
}