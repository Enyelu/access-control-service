using access_control.core.Commands.Lock;
using access_control.core.DataTransferObjects;
using access_control.core.Queries.Lock;
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
        public async Task<IActionResult> OpenLock([FromHeader]Guid lockId)
        {
            var response = await Mediator.Send(new HandleOpenLock.Command
            {
                LockId = lockId,
                RoleId = GetRequiredValues().roleId,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpPost("close")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> CloseLock([FromHeader]Guid lockId)
        {
            var response = await Mediator.Send(new HandleCloseLock.Command
            {
                LockId = lockId,
                RoleId = GetRequiredValues().roleId,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpPost("complaint")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> RaiseComplaint([FromBody] string request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("view-complaint")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> ViewComplaint(string request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("allocated")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> FetchAllocatedLocks()
        {
            var response = await Mediator.Send(new HandleFetchAllocatedLocks.Query
            {
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }

        [HttpGet("assigned")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> FetchAssignedLocks()
        {
            var response = await Mediator.Send(new HandleFetchAssignedLocks.Query
            {
                RoleId = GetRequiredValues().roleId,
                TenantId = GetRequiredValues().tenantId
            });
            return Ok(response);
        }
    }
}
