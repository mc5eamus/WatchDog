using Microsoft.Extensions.Logging;
using WatchDog.Contract;

namespace WatchDog.Publishers
{
    /// <summary>
    /// Example of a base publisher imitating a long-running operation
    /// </summary>
    public class BasePublisher : IPublisher
    {
        private readonly ILogger logger;

        public BasePublisher(ILogger logger)
        {
            this.logger = logger;
        }

        protected ILogger Logger => logger;

        /// <summary>
        /// Implements the IPublisher and imitates a long-running send operation.
        /// </summary>
        /// <typeparam name="TModel">Payload type</typeparam>
        /// <param name="model">Payload</param>
        /// <param name="token">Cancellation token</param>
        /// <returns></returns>
        public async Task Publish<TModel>(TModel model, CancellationToken token) where TModel : class
        {
            int delay = 1 + Random.Shared.Next(8);

            logger.LogInformation($"{this.GetType().Name} will be running for {delay}s");

            await Task.Delay(1000 * delay, token);

            if (token.IsCancellationRequested)
            {
                logger.LogInformation($"cancelled publishing {model} (probably on timeout)");
            }
            else
            {
                logger.LogInformation($"done publishing {model}");
            }

            await Task.CompletedTask;
        }
    }
}
