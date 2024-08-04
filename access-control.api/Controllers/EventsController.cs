using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace access_control.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ApiBaseController
    {
        private readonly IMapper _mapper;
        public EventsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> LockEvent([FromQuery] string request)
        {
            var response = await Mediator.Send(new object());
            return Ok(response);
        }
    }
}
