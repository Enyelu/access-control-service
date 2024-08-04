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
        public async Task<IActionResult> CreateLock([FromQuery] string email)
        {
            var response = await Mediator.Send(email);
            return Ok(response);
        }

        [HttpPatch("allocate-lock")]
        public async Task<IActionResult> AllocateLock([FromBody]string request)
        {
            var response = await Mediator.Send(request);
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
        public async Task<IActionResult> ViewComplaint([FromBody] string request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
