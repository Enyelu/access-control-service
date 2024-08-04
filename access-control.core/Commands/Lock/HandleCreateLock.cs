using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EntityModels = access_control.domain.Entities;

namespace access_control.core.Commands.Lock
{
    public class HandleCreateLock
    {
        public class Command : IRequest<GenericResponse<string>>
        {
            public string Name { get; set; }
            public string UserId { get; set; }
            public string SerialNumber { get; set; }
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
                _logger.LogError($"Attempting to create a new lock with name: {request.Name}, serial number: {request.SerialNumber}");

                var existingLock = await _dbContext.Locks
                    .Where(x => x.Name == request.Name || x.SerialNumber == request.SerialNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingLock != null)
                    return GenericResponse<string>.Fail("Duplicate entry detected", 400);

                var newLock = _mapper.Map<EntityModels.Lock>(request, options =>
                {
                    options.Items["CreatedBy"] = request.UserId;
                });

                await _dbContext.Locks.AddAsync(newLock, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return GenericResponse<string>.Success("Success", "Lock created successfully");
            }
        }
    }
}
