using access_control.core.Commands.Lock;
using access_control.core.DataTransferObjects;
using access_control.core.Queries.Lock;
using access_control.core.Shared;
using AutoMapper;
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
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> RaiseComplaint([FromBody] RaiseComplaintDto request)
        {
            var mappedRequest = _mapper.Map<HandleRaiseComplaint.Command>(request);
            var response = await Mediator.Send(mappedRequest);
            return Ok(response);
        }

        [HttpGet("view-complaint")]
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
        public async Task<IActionResult> FetchAllocatedLocks()
        {
            var response = await Mediator.Send(new HandleFetchAllocatedLocks.Query
            {
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpGet("assigned")]
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
