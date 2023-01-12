using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Model;

namespace WatchDog.Queries
{
    /// <summary>
    /// Example of a query as part of a watchdog, spitting out <see cref="ExtendedModel">ExtendedModel</see>
    /// </summary>
    public class SomeOtherQuery : IQuery<ExtendedModel>
    {
        ILogger logger;
        SomeOtherQueryConfig config;

        public SomeOtherQuery(ILogger<SomeOtherQuery> logger, IOptions<SomeOtherQueryConfig> config) { 
            this.logger = logger;
            this.config = config.Value;
            logger.LogInformation($"... using {this.config.SomeOtherServiceConnectionString}");
        }

        public async Task<ExtendedModel> Execute(CancellationToken ctoken)
        {
            int delay = 1 + Random.Shared.Next(10);
            logger.LogInformation($"{this.GetType().Name} is running for {delay}s");
            await Task.Delay(1000 * delay, ctoken);

            return await Task.FromResult(new ExtendedModel() { 
                Id = 2,
                SomeProperty = $"here we go, {DateTime.Now.ToShortTimeString()}"
            });
        }
    }
}
