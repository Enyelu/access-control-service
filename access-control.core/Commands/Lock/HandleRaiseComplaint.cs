using access_control.core.Shared;
using access_control.domain.Entities;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace access_control.core.Commands.Lock
{
    public class HandleRaiseComplaint
    {
        public class Command : IRequest<GenericResponse<string>>
        {
            public string Email { get; set; }
            public string UserId { get; set; }
            public string Subject { get; set; }
            public string Message { get; set; }
        }
        public class Handler : IRequestHandler<Command, GenericResponse<string>>
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
            public async Task<GenericResponse<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.LogError($"Logging compaint by {request.Email}");

                var complaint = _mapper.Map<Complaint>(request, options =>
                {
                    options.Items["CreatedBy"] = request.UserId;
                });

                await _dbContext.Complaints.AddAsync(complaint, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return GenericResponse<string>.Success("Success", "Complaint created successfully");
            }
        }
    }
}
