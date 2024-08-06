using access_control.domain.Entities;
using access_control.infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;

namespace access_control.api.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationContext dbContext)
        {
            // Saving original response stream
            var originalBodyStream = context.Response.Body;

            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;
                var endpoint = context.GetEndpoint();
                var routeEndpoint = endpoint as RouteEndpoint;
                var actionName = routeEndpoint?.RoutePattern.RequiredValues["action"]?.ToString();
                var userId = context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value?.ToString();
                var tenantId = context?.User?.Claims.FirstOrDefault(x => x.Type == "TenantId")?.Value?.ToString();

                try
                {
                    // Process the request
                    await _next(context);

                    // Capture the response
                    var responseBody = await ReadResponseBody(context.Response);
                    var serviceResponse = GetProcessStatusCode(responseBody);
                    var serviceStatusCode = serviceResponse.statusCode > 0 ? serviceResponse.statusCode : 000;
                    var serviceResponseMessage = serviceResponse.Message;

                    // Log the request and response
                    var eventLog = new EventLog
                    {
                        TenantId = tenantId,
                        CreatedBy = userId,
                        Action = actionName,
                        Changes = JsonConvert.SerializeObject(new
                        {
                            Message = serviceResponseMessage,
                            RequestMethod = context.Request.Method,
                            RequestQueryString = context.Request.QueryString.ToString(),
                            RequestBody = await ReadRequestBody(context.Request),
                            ResponseStatusCode = serviceStatusCode,
                            ResponseBody = responseBody,
                        }),
                        IsSuccessful = serviceStatusCode == 200 || serviceStatusCode == 201,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.EventLogs.Add(eventLog);
                    await dbContext.SaveChangesAsync();

                    // Write the response back to the original stream
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    var eventLog = new EventLog
                    {
                        TenantId = tenantId,
                        Action = actionName,
                        CreatedBy = userId,
                        Changes = JsonConvert.SerializeObject(new { ErrorMessage = ex.Message, ErrorStackTrace = ex.StackTrace }),
                        IsSuccessful = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    dbContext.EventLogs.Add(eventLog);
                    await dbContext.SaveChangesAsync();

                    throw;
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return responseBody;
        }

        private static (int statusCode, string Message) GetProcessStatusCode(string response)
        {
            try
            {
                JObject json = JObject.Parse(response);
                var statusCode = (int)json["statusCode"];
                var message = (string)json["message"];
                return (statusCode, message);
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}
