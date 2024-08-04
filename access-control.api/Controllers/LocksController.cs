using access_control.core.Commands.Lock;
using access_control.core.DataTransferObjects;
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
        public async Task<IActionResult> OpenLock([FromBody] string request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("close")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> CloseLock([FromBody] string request)
        {
            var response = await Mediator.Send(request);
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

        [HttpGet("{tenantId}")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> FetchLockByTenantId([FromRoute] string tenantId)
        {
            var response = await Mediator.Send(tenantId);
            return Ok(response);
        }
    }
}
