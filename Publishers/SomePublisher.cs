using Microsoft.Extensions.Logging;
using WatchDog.Contract;

namespace WatchDog.Publishers
{
    public class SomePublisher : IPublisher
    {
        protected readonly ILogger<SomePublisher> logger;

        public SomePublisher(ILogger<SomePublisher> logger) { 
            this.logger = logger;
        }
        
        public async Task Publish<TModel>(TModel model, CancellationToken token) where TModel : class
        {
            int delay = 1 + Random.Shared.Next(10);
            logger.LogInformation($"{this.GetType().Name} will be running for {delay}s");

            await Task.Delay(1000*delay);

            if(token.IsCancellationRequested)
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
