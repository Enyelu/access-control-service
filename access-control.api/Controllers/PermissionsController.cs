using access_control.core.Commands.Permission;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace access_control.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ApiBaseController
    {
        private readonly IMapper _mapper;
        public PermissionsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] HandleCreatePermission.Command request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission([FromBody] string request)
        {
            var response = await Mediator.Send(new object());
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> PermissionByTenantId([FromBody] string request)
        {
            var response = await Mediator.Send(new object());
            return Ok(response);
        }
    }
}