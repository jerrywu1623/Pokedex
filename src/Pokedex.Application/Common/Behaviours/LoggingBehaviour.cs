using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pokedex.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        const string REQUEST_ID = "Request-Id";

        public LoggingBehaviour(ILogger<TRequest> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestName = request.GetType().Name;
            TResponse response;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                {REQUEST_ID, _httpContextAccessor.HttpContext.TraceIdentifier}
            }))
            {
                _logger.LogInformation($"[START] {requestName}");

                try
                {
                    _logger.LogInformation($"[PROPS] {JsonSerializer.Serialize(request)}");
                    response = await next();
                    _logger.LogInformation($"[RES] {JsonSerializer.Serialize(response)}");
                }
                finally
                {
                    _logger.LogInformation($"[END] {requestName}");
                }
            }
            return response;
        }
    }
}