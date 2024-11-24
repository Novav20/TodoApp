using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace TodoApp.Web.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthenticationHandler> _logger;

        public AuthenticationHandler(IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
            
            _logger.LogDebug("Request to {Url}", request.RequestUri);
            
            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("Adding token to request");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("No token found for request to {Url}", request.RequestUri);
            }

            var response = await base.SendAsync(request, cancellationToken);
            
            _logger.LogDebug("Response from {Url}: {StatusCode}", request.RequestUri, response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Error response from {Url}: {StatusCode} - {Content}", 
                    request.RequestUri, response.StatusCode, content);
            }

            return response;
        }
    }
}
