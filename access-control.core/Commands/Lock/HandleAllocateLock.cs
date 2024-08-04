using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace access_control.core.Commands.Lock
{
    public class HandleAllocateLock
    {
        public class Command : IRequest<GenericResponse<string>>
        {
            public string UserId { get; set; }
            public Guid LockId { get; set; }
            public string TenantId { get; set; }
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
                _logger.LogError($"Attempting to allocate lock with id: {request.LockId}");

                var existingLock = await _dbContext.Locks
                    .Where(x => x.Id == request.LockId.ToString())
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingLock == null)
                    return GenericResponse<string>.Fail("Lock not found");

                if (existingLock.TenantId != null)
                    return GenericResponse<string>.Fail("Lock already allocated");

                existingLock.TenantId = request.TenantId;
                existingLock.CreatedBy = request.UserId;
                 _dbContext.Locks.Update(existingLock);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return GenericResponse<string>.Success("Success", "Lock allocated successfully");
            }
        }
    }
}
