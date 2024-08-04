using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EntityModels = access_control.domain.Entities;

namespace access_control.core.Commands.Permission
{
    public class HandleCreatePermission
    {
        public class Command : IRequest<GenericResponse<string>>
        {
            public Guid UserId { get; set; }
            public Guid TenantId { get; set; }
            public List<CommandExtention> Instructions { get; set; }
            
        }

        public class CommandExtention
        {
            public Guid RoleId { get; set; }
            public Guid LockId { get; set; }
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
                _logger.LogError($"Attempting to create permission with instructions {JsonConvert.SerializeObject(request.Instructions)} by {request.UserId} at {DateTime.UtcNow}");

                var RoleIds = request.Instructions.Select(x => x.RoleId.ToString() ).ToList();
                var LockIds = request.Instructions.Select(x => x.LockId.ToString()).ToList();
                var tenantId = request.TenantId.ToString();

                var fraudCheck = await _dbContext.Permissions.Where(x => LockIds.Contains(x.LockId) && x.TenantId != tenantId).ToListAsync();
                if(fraudCheck.Any())
                    return GenericResponse<string>.Fail("Unauthorized", 401);

                var existingPermissions = await _dbContext.Permissions.Where(x => 
                                    LockIds.Contains(x.LockId) && 
                                    RoleIds.Contains(x.RoleId) && 
                                    x.TenantId == tenantId).ToListAsync();

                var newPermissions = request.Instructions
                                    .Where(x => !existingPermissions.Any(ep => 
                                    ep.LockId == x.LockId.ToString() && 
                                    ep.RoleId == x.RoleId.ToString() && 
                                    ep.TenantId == tenantId))
                                    .ToList();

                if(newPermissions == null || !newPermissions.Any())
                    return GenericResponse<string>.Fail("Specified permission(s) mapping alreay exist", 400);


                var permissions = _mapper.Map<List<EntityModels.Permission>>(newPermissions, options =>
                {
                    options.Items["TenantId"] = tenantId;
                    options.Items["CreatedBy"] = request.UserId.ToString();
                });
                await _dbContext.Permissions.AddRangeAsync(permissions);
                _dbContext.SaveChanges();

                _logger.LogError($"Create permission with instructions {JsonConvert.SerializeObject(request.Instructions)} by {request.UserId} at {DateTime.UtcNow} successfully");
                return GenericResponse<string>.Success("Success", "Permission(s) created successfully");
            }
        }
    }
}
