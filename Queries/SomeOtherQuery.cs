using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Model;

namespace WatchDog.Queries
{
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
