using Polly;
using Polly.Extensions.Http;

namespace DevelopersApi;

public static class BasicPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetyPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircutBreakerPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(1, TimeSpan.FromSeconds(30));
    }
}
