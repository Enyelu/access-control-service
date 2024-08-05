using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace access_control.core.Queries.Lock
{
    public class HandleViewComplaint
    {
        public class Query : IRequest<GenericResponse<PaginationResult<Result>>>
        {
            public DateTime End { get; set; }
            public DateTime Start { get; set; }
            public int PageSize { get; set; }
            public int PageNumber { get; set; }
        }

        public class Result
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }
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
                    request.Start = request.End.AddMonths(-1);

                var complaints = await _dbContext.Complaints
                    .Where(x => x.CreatedAt >= request.Start && x.CreatedAt <= request.End)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync(cancellationToken);

                if (!complaints.Any())
                    GenericResponse<string>.Fail($"No complaint(s) found between {request.Start} and {request.End}");

                var result = _mapper.Map<List<Result>>(complaints);
                var pagedResult = Paginator<Result>.GetPagedData(result, request.PageNumber, request.PageSize);
                return GenericResponse<PaginationResult<Result>>.Success(pagedResult, "successful");
            }
        }
    }
}
