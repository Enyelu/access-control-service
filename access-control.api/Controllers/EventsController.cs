using access_control.core.Queries.Event;
using access_control.core.Shared;
using access_control.domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "TenantSuperAdmin,TenantAdmin,SuperAdmin,Admin")]
        [ProducesResponseType(typeof(GenericResponse<PaginationResult<HandleFetchEvents.Result>>), 200)]
        public async Task<IActionResult> FetchEvents([FromHeader]Guid tenantId, [FromQuery]DateTime start, [FromQuery]DateTime end, 
            [FromQuery]EventEnum eventType = EventEnum.Generic, int pageSize = 20, int pageNumber = 1)
        {
            if (tenantId != Guid.Empty && User.IsInRole("TenantSuperAdmin") || User.IsInRole("TenantAdmin"))
                return Unauthorized();

            var actualTenantId = tenantId == Guid.Empty ? GetRequiredValues().tenantId : tenantId.ToString();
            var response = await Mediator.Send(new HandleFetchEvents.Query 
            {
                End = end,
                Start = start,
                PageSize = pageSize,
                PageNumber = pageNumber,
                EventType = eventType,
                TenantId = actualTenantId
            });
            return Ok(response);
        }
    }
}