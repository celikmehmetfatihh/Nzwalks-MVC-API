using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger,
            RequestDelegate next) // returns task that represents the completion of a request processing
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // if anything happends during the calls, handle the exceptions at a single location 
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                // Log this exception
                _logger.LogError(ex, $"{errorId} : {ex.Message}");

                // Return a custom error response
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong! We are looking into resolving this."
                };

                await httpContext.Response.WriteAsJsonAsync(error);
            }

        }
    }
}
