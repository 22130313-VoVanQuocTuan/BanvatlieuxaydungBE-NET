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
                // Kiểm tra xem phản hồi đã bắt đầu chưa
                if (!httpContext.Response.HasStarted)
                {
                    // Thay đổi header Content-Type chỉ khi phản hồi chưa bắt đầu
                    httpContext.Response.ContentType = "application/json";

                    // Viết dữ liệu JSON vào phản hồi
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
                            status = 401,
                            message = "Không có quyền truy cập"
                        }
                        ));

                }
            }
        }
    }
}

        
    

