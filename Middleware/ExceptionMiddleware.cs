using System.Text.Json;
using Licentra.API.Exceptions;
using Licentra.API.Exceptions.Custom;

namespace Licentra.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var response = new ErrorResponse
            {
                Success = false,
                Message = exception.Message
            };

            switch (exception)
            {
                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;

                case ConflictException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    break;

                case BadRequestException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case UnauthorizedException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An unexpected error occurred.";
                    break;
            }

            response.StatusCode = context.Response.StatusCode;

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}