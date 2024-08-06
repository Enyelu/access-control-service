using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace access_control.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected (string userId, string tenantId, string staffId, List<string> roleIds) GetRequiredValues()
        {
            var staffId = User?.Claims?.FirstOrDefault(x => x.Type == "StaffId")?.Value.ToString();
            var tenantId = User?.Claims.FirstOrDefault(x => x.Type == "TenantId")?.Value?.ToString();
            var userId = User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value.ToString();

            var roleDefinitions = User?.Claims?.Where(x => x.Type == "RoleDefinitions")?.ToList();

            var roleIds = new List<string>();
            foreach (var item in roleDefinitions)
            {
                roleIds.Add(item.Value?.Split(":")?[1]?.ToString());
            }

            if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId) || roleDefinitions == null || !roleDefinitions.Any())
                throw new Exception("Unauthorized");
            return (userId, tenantId, staffId, roleIds);
        }
    }
}
