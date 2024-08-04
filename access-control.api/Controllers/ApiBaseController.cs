using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace access_control.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected (string userId, string tenantId) GetRequiredValues()
        {
            var userId = User?.Identity?.Name;
            var tenantId = User?.FindFirst("TenantId")?.ToString();
            /*if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("UserId is required");*/
            return (userId, tenantId);
        }
    }
}
