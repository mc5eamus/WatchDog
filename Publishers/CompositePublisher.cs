using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using System.Runtime.CompilerServices;
using WatchDog.Contract;
using WatchDog.Resiliency;

namespace WatchDog.Publishers
{
    /// <summary>
    /// Example of a composite publisher taking care of 2 specific publishers
    /// </summary>
    public class CompositePublisher : IPublisher
    {
        private ICollection<IPublisher> publishers;

        private readonly AsyncRetryPolicy retryPolicy;
        private readonly AsyncTimeoutPolicy timeoutPolicy;

        /// <summary>
        /// As long as the list of involved publishers is known ahead of time, 
        /// we simply let DI pass them over as contructor parameters.
        /// We also implement resiliency in this class instead of letting individual publishers
        /// take care of it for demonstration purposes only
        /// </summary>
        /// <param name="policies">Polly policy registry to add resiliency to underlying calls</param>
        /// <param name="p1">Publisher 1</param>
        /// <param name="p2">Publisher 2</param>
        public CompositePublisher(IPolicyRegistry<string> policies, SomePublisher p1, SomeOtherPublisher p2) {
            publishers = new List<IPublisher> { p1, p2 };

            retryPolicy = policies
                .Get<AsyncRetryPolicy>(PolicyConstants.SIMPLE_RETRY_POLICY);
            timeoutPolicy = policies
                    .Get<AsyncTimeoutPolicy>(PolicyConstants.SHORT_TIMEOUT_POLICY);

        }

        /// <summary>
        /// Implements the IPublisher and sends the payload to 2 publishers.
        /// In a realistic scenario a message/event oriented approach would be more appropriate
        /// </summary>
        /// <typeparam name="TModel">Payload type</typeparam>
        /// <param name="model">Payload</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task Publish<TModel>(TModel model, CancellationToken token) where TModel : class
        {
            foreach (var publisher in publishers)
            {
                await retryPolicy
                    .ExecuteAsync(async _ => await timeoutPolicy
                        .ExecuteAsync(async ctoken => await publisher.Publish(model, ctoken), token), token);  
            }
            
        }
    }
}
