using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using Polly.Timeout;

namespace WatchDog.Resiliency
{
    public static class PolicyConstants
    {
        public const string SIMPLE_RETRY_POLICY = "SimpleRetryPolicy";
        public const string SHORT_TIMEOUT_POLICY = "ShortTimeoutPolicy";
    }

    /// <summary>
    /// Creates a policy registry for downstream services to use
    /// </summary>
    public static class PolicyExtensions
    {
        public static IServiceCollection AddResiliencyPolicyRegistry(this IServiceCollection services)
        {
            IPolicyRegistry<string> registry = services.AddPolicyRegistry();

            // one to restict the execution time of individual steps
            registry.Add(PolicyConstants.SHORT_TIMEOUT_POLICY,
                Policy.TimeoutAsync(TimeSpan.FromSeconds(5), TimeoutStrategy.Optimistic));

            // one to handle the case where the step doesn't like to be violently terminated
            // (do it 3 times and then give up)
            registry.Add(PolicyConstants.SIMPLE_RETRY_POLICY, 
                Policy
                    .Handle<TimeoutRejectedException>()
                    .WaitAndRetryAsync(3, 
                    retryAttempt => TimeSpan.FromSeconds(retryAttempt) ));

            return services;
        }
    }
}
