using Newtonsoft.Json;

namespace AcountService.AppException
{
    public class StatusErrorCode
    {
        private readonly RequestDelegate _next;

        public StatusErrorCode(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);

            if (httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {

                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                    new
                    {
                        status = 401,
                        message = "Không có quyền truy cập"
                    }
                    ));

            }
            if (httpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
            {

                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                    new
                    {
                        status = 403,
                        message = "Không có quyền truy cập"
                    }
                    ));

            }
        }
    }
}

        
    

