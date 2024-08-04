using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ProgramGuard.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case ApiException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.ErrorCode = "API_ERROR";
                        break;
                    case ValidationException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        responseModel.ErrorCode = "VALIDATION_ERROR";
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.ErrorCode = "NOT_FOUND";
                        break;
                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseModel.ErrorCode = "UNAUTHORIZED";
                        break;
                    default:
                        // unhandled error
                        _logger.LogError(error, "Unhandled exception");
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.ErrorCode = "INTERNAL_SERVER_ERROR";
                        responseModel.Message = _env.IsDevelopment() ? error?.Message : "An unexpected error occurred.";
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
