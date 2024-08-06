using access_control.core.Commands.Lock;
using access_control.core.DataTransferObjects;
using access_control.core.Queries.Lock;
using access_control.core.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace access_control.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocksController : ApiBaseController
    {
        private readonly IMapper _mapper;
        public LocksController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> CreateLock([FromBody]CreateLockDto request)
        {
            var response = await Mediator.Send(new HandleCreateLock.Command
            {
                Name = request.Name,
                UserId = GetRequiredValues().userId,
                SerialNumber = request.SerialNumber

            });
            return Ok(response);
        }

        [HttpPatch("allocate-lock")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> AllocateLock([FromHeader] Guid lockId)
        {
            var response = await Mediator.Send(new HandleAllocateLock.Command
            {
                LockId = lockId,
                UserId = GetRequiredValues().userId,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpPost("open")]
        [Authorize]
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> OpenLock([FromHeader]Guid lockId)
        {
            var response = await Mediator.Send(new HandleOpenLock.Command
            {
                LockId = lockId,
                RoleIds = GetRequiredValues().roleIds,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpPost("close")]
        [Authorize]
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> CloseLock([FromHeader]Guid lockId)
        {
            var response = await Mediator.Send(new HandleCloseLock.Command
            {
                LockId = lockId,
                RoleIds = GetRequiredValues().roleIds,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpPost("complaint")]
        [Authorize]
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> RaiseComplaint([FromBody] RaiseComplaintDto request)
        {
            var mappedRequest = _mapper.Map<HandleRaiseComplaint.Command>(request);
            mappedRequest.UserId = GetRequiredValues().userId;
            var response = await Mediator.Send(mappedRequest);
            return Ok(response);
        }

        [HttpGet("view-complaint")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> ViewComplaint([FromQuery] DateTime start, [FromQuery] DateTime end, int pageSize = 20, int pageNumber = 1)
        {
            var response = await Mediator.Send(new HandleViewComplaint.Query 
            { 
                Start = start, 
                End = end, 
                PageNumber = pageNumber, 
                PageSize = pageSize
            });
            return Ok(response);
        }

        [HttpGet("allocated")]
        [Authorize(Roles = "TenantSuperAdmin,TenantAdmin,SuperAdmin,Admin")]
        public async Task<IActionResult> FetchAllocatedLocks(string tenantId)
        {
            if(!string.IsNullOrWhiteSpace(tenantId) && User.IsInRole("TenantSuperAdmin") || User.IsInRole("TenantAdmin"))
                return Unauthorized();

            var actualTenantId = string.IsNullOrWhiteSpace(tenantId) ? GetRequiredValues().tenantId : tenantId;
            var response = await Mediator.Send(new HandleFetchAllocatedLocks.Query
            {
                TenantId = actualTenantId
            });
            return Ok(response);
        }

        [HttpGet("assigned")]
        [Authorize(Roles = "TenantSuperAdmin,TenantAdmin")]
        public async Task<IActionResult> FetchAssignedLocks()
        {
            var response = await Mediator.Send(new HandleFetchAssignedLocks.Query
            {
                RoleIds = GetRequiredValues().roleIds,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }
    }
}
