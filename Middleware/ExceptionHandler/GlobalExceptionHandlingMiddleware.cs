using System.Text.Json;
using APICacheWithRedis.Payload;

namespace APICacheWithRedis.Middleware.ExceptionHandler{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            // var response = context.Response;
            Response<Object> exModel = new Response<object>();

            switch (exception)
            {
                case ApplicationException ex:
                    exModel.Success = false;
                    exModel.Message = $"ERROR {ex.Message}";
                    exModel.Error = "Application Exception Occured, please retry after sometime.";
                    _logger.LogError(ex, "Error Application {@Response}", exModel);
                    break;
                case FileNotFoundException ex:
                    exModel.Success = false;
                    exModel.Message = $"ERROR {ex.Message }";
                    exModel.Error = "The requested resource is not found.";
                    _logger.LogError(ex, "Error File Exception {@Response}", exModel);
                    break;
                default:
                    exModel.Success = false;
                    exModel.Message = "ERROR";
                    exModel.Error = "Internal Server Error, Please retry after sometime";
                    _logger.LogError("Error {@Response}", exModel);
                    break;

            }
            var exResult = JsonSerializer.Serialize(exModel);
            await context.Response.WriteAsync(exResult);
        }
    }
}