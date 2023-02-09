using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDog.Common;
using WatchDog.Config;
using WatchDog.Contract;

namespace WatchDog.Jobs
{
    /// <summary>
    /// A watchdog job executing a query of type TQuery (returning an object of type TModel)
    /// </summary>
    /// <typeparam name="TQuery">Query type to execute</typeparam>
    /// <typeparam name="TModel">Model returned by the query</typeparam>
    public class MulticonfigWatchdog<TDataSource, TConfig, TModel> : ScheduledService
        where TDataSource : IDataSource<TModel, TConfig>
        where TConfig : class
        where TModel : class
    {
        private readonly TDataSource dataSource;
        private readonly ILogger logger;
        private readonly ScheduleConfig<TDataSource> scheduleConfig;
        private readonly IEnumerable<TConfig> dataSourceConfig;
        private readonly IPublisher publisher;

        /// <summary>
        /// All parameters of the Generic Watchdog are supposed to be implicitly provided by DI
        /// </summary>
        /// <param name="query">Query to execute incl. the output type as parameter</param>
        /// <param name="logger">Logger (we're requesting a specific type for the class</param>
        /// <param name="config">Config for scheduling (don't confuse with Query settings like connection strings</param>
        public MulticonfigWatchdog(TDataSource dataSource, 
            IPublisher publisher, 
            ILogger<MulticonfigWatchdog<TDataSource, TConfig, TModel>> logger,
            IOptions<ScheduleConfig<TDataSource>> scheduleConfig,
            IOptions<IEnumerable<TConfig>> dataSourceConfig
            ) : base (logger, scheduleConfig)
        {
            this.dataSource = dataSource;
            this.scheduleConfig = scheduleConfig.Value;
            this.dataSourceConfig = dataSourceConfig.Value;
            this.publisher = publisher;
            this.logger = logger;
            logger.LogInformation($"{this.scheduleConfig.CronExpression} for {typeof(TDataSource).Name}");
        }

        /// <summary>
        /// The actual workload and implementation for the ScheduledService, will be called when the execution is due
        /// accorting to the schedule, will execute the worker for all assigned configurations
        /// </summary>
        /// <param name="cancellationToken">cancellation token for termination, use consistently in all downstream async operations</param>
        /// <returns>async void</returns>
        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"{this.GetType().Name} started");

            // iterates through all configs scheduled to run
            foreach(var dsConfig in dataSourceConfig)
            {
                TModel queryResult = await dataSource.Execute(dsConfig, cancellationToken);
                await publisher.Publish(queryResult, cancellationToken);
                Logger.LogInformation($"{queryResult}");
            }
        }
    }
}
