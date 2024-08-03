using access_control.domain.Entities;
using access_control.infrastructure;
using Newtonsoft.Json;
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

                try
                {
                    // Process the request
                    await _next(context);

                    // Capture the response
                    var responseBody = await ReadResponseBody(context.Response);

                    // Log the request and response
                    var eventLog = new EventLog
                    {
                        CreatedBy = context.User?.Identity?.Name ?? "Anonymous",
                        Action = "Request",
                        Changes = JsonConvert.SerializeObject(new
                        {
                            RequestMethod = context.Request.Method,
                            RequestQueryString = context.Request.QueryString.ToString(),
                            RequestHeaders = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                            RequestBody = await ReadRequestBody(context.Request),
                            ResponseStatusCode = context.Response.StatusCode,
                            ResponseBody = responseBody
                        }),
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
                        Action = "System",
                        CreatedBy = "Error",
                        Changes = JsonConvert.SerializeObject(new { ErrorMessage = ex.Message, ErrorStackTrace = ex.StackTrace }),
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
    }
}
