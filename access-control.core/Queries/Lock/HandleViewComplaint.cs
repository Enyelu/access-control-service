using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace access_control.core.Queries.Lock
{
    public class HandleViewComplaint
    {
        public class Query : IRequest<GenericResponse<string>>
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
        public class Handler : IRequestHandler<Query, GenericResponse<string>>
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
            public async Task<GenericResponse<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                _logger.LogError($"");


                return GenericResponse<string>.Success("Success", "Permission deleted created successfully");
            }
        }
    }
}
