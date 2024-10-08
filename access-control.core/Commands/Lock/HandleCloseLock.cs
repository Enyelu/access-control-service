﻿using access_control.core.Shared;
using access_control.infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace access_control.core.Commands.Lock
{
    public class HandleCloseLock
    {
        public class Command : IRequest<GenericResponse<string>>
        {
            public string TenantId { get; set; }
            public List<string> RoleIds { get; set; }
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
                _logger.LogError($"Attempting to close lock {request.LockId}");
                string lockId = request.LockId.ToString();

                var lockToClose = await _dbContext.Locks.Where(x => x.Id == lockId).FirstOrDefaultAsync(cancellationToken);
                if (lockToClose == null)
                    return GenericResponse<string>.Fail("Lock not found");

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(x =>
                x.LockId == lockId &&
                x.TenantId == request.TenantId &&
                request.RoleIds.Contains(x.RoleId), cancellationToken);

                if (permission == null)
                    return GenericResponse<string>.Fail("Permission denied", 401);

                bool apiCallToMicrocontroller = true; //Simulating value to be true
                //Make API call to open lock (typically a micro controller such as nanoframework that opens the lock)

                //If lock failed to close then
                if (!apiCallToMicrocontroller)
                    return GenericResponse<string>.Fail("Something happened, please try again!", 401);

                //If lock closed successfully then
                lockToClose.IsOpen = false;
                lockToClose.OpenedAt = null;
                _dbContext.Update(lockToClose);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return GenericResponse<string>.Success("Success", "Lock closed successfully!");
            }
        }
    }
}
