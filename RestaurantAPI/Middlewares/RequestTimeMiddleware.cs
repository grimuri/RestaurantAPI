using System.Diagnostics;

namespace RestaurantAPI.Middlewares
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private Stopwatch _stopWatch;
        private readonly ILogger _logger;

        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            _logger = logger;
            _stopWatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopWatch.Start();
            await next.Invoke(context);
            _stopWatch.Stop();

            var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
            if(elapsedMilliseconds / 1000 > 4)
            {
                var message =
                    $"Request {context.Request.Method} at {context.Request.Path} took {elapsedMilliseconds} ms";
                _logger.LogInformation(message);
            }
        }
    }
}
