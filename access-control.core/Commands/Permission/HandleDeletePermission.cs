using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace access_control.core.Commands.Permission
{
    public class HandleDeletePermission
    {
        public class Command : IRequest<GenericResponse<string>>
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public Guid LockId { get; set; }
            public Guid TenantId { get; set; }
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
                _logger.LogError($"Attempting to delete permission with Id {request.Id} by {request.UserId} at {DateTime.UtcNow}");

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(x => 
                x.Id == request.Id.ToString() && 
                x.LockId == request.LockId.ToString() &&
                x.TenantId == request.TenantId.ToString());

                if (permission == null)
                    return GenericResponse<string>.Fail("Permission does not exist", 404);

                _dbContext.Permissions.Remove(permission);
                _dbContext.SaveChanges();

                return GenericResponse<string>.Success("Success", "Permission deleted created successfully");
            }
        }
    }
}