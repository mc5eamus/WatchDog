using Microsoft.Extensions.Logging;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using System.Reflection;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Model;
using WatchDog.Resiliency;

namespace WatchDog.Queries
{
    /// <summary>
    /// Example of a data source executing a query based on provided configuration and returning an instance of the model
    /// </summary>
    public class SomeDataSource : IDataSource<BaseModel, SomeDataSourceConfig>
    {
        ILogger logger;
        private readonly AsyncRetryPolicy retryPolicy;
        private readonly AsyncTimeoutPolicy timeoutPolicy;

        public SomeDataSource(IPolicyRegistry<string> policies, ILogger<SomeDataSource> logger) { 
            retryPolicy = policies
                .Get<AsyncRetryPolicy>(PolicyConstants.SIMPLE_RETRY_POLICY);
            timeoutPolicy = policies
                    .Get<AsyncTimeoutPolicy>(PolicyConstants.SHORT_TIMEOUT_POLICY);
            this.logger = logger;
        }

        /// <summary>
        /// Implements the IDataSource contract of executing a query and responding with TModel
        /// </summary>
        /// <param name="config">Instance of datasource/query configuration</param>
        /// <param name="ctoken">Cancellation token to allow termination of long-running async processes</param>
        /// <returns></returns>
        public async Task<BaseModel> Execute(SomeDataSourceConfig config, CancellationToken ctoken)
        {
            // just an example of how to use resiliency policies for worker calls
            return await retryPolicy
                .ExecuteAsync(async _ => await timeoutPolicy
                    .ExecuteAsync(async ctoken => {
                        int delay = 1 + Random.Shared.Next(5);
                        logger.LogInformation($"{this.GetType().Name} will be running for {delay}s, {config.SomeServiceConnectionString}");
                        await Task.Delay(1000 * delay, ctoken);
                        return await Task.FromResult(new BaseModel() { Id = Random.Shared.Next() });
                    }, ctoken), ctoken);
        }
    }
}
