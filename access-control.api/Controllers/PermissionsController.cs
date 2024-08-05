using access_control.core.Commands.Permission;
using access_control.core.DataTransferObjects;
using access_control.core.Queries.Permission;
using access_control.core.Shared;
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
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> CreatePermission([FromBody] HandleCreatePermission.Command request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(GenericResponse<string>), 200)]
        public async Task<IActionResult> DeletePermission([FromBody] DeletePermissionDto request)
        {
            var mappedRequest = _mapper.Map<HandleDeletePermission.Command>(request);
            var response = await Mediator.Send(mappedRequest);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> PermissionByTenantId([FromHeader] Guid tenantId, [FromQuery] DateTime start,
            [FromQuery]DateTime end, [FromQuery]int PageSize = 20, [FromQuery]int PageNumber = 1)
        {
            var response = await Mediator.Send(new HandlePermissionByTenantId.Query 
            { 
                TenantId = tenantId,
                Start = start,
                End = end,
                PageNumber = PageNumber,
                PageSize = PageSize
            });
            return Ok(response);
        }
    }
}