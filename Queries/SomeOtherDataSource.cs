using Microsoft.Extensions.Logging;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Model;

namespace WatchDog.Queries
{
    public class SomeOtherDataSource : IDataSource<ExtendedModel, SomeOtherDataSourceConfig>
    {
        ILogger logger;

        public SomeOtherDataSource(ILogger<SomeOtherDataSource> logger) { 
            this.logger = logger;
        }

        public async Task<ExtendedModel> Execute(SomeOtherDataSourceConfig config, CancellationToken ctoken)
        {
            int delay = 1 + Random.Shared.Next(5);
            logger.LogInformation($"{this.GetType().Name} is running for {delay}s, {config.SomeOtherServiceConnectionString}");
            await Task.Delay(1000 * delay, ctoken);
            return await Task.FromResult(new ExtendedModel() { Id = Random.Shared.Next() });
        }
    }
}