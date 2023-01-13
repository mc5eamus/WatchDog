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
    public class GenericWatchdog<TQuery, TModel> : ScheduledService
        where TQuery : IQuery<TModel>
        where TModel : class
    {
        private readonly TQuery query;
        private readonly ILogger logger;
        private readonly IScheduleConfig config;
        private readonly IPublisher publisher;

        /// <summary>
        /// All parameters of the Generic Watchdog are supposed to be implicitly provided by DI
        /// </summary>
        /// <param name="query">Query to execute incl. the output type as parameter</param>
        /// <param name="logger">Logger (we're requesting a specific type for the class</param>
        /// <param name="config">Config for scheduling (don't confuse with Query settings like connection strings</param>
        public GenericWatchdog(TQuery query, 
            IPublisher publisher, 
            ILogger<TQuery> logger, 
            IOptions<ScheduleConfig<TQuery>> config) : base (logger, config)
        {
            this.query = query;
            this.publisher = publisher;
            this.logger = logger;
            this.config = config.Value;
            logger.LogInformation($"{this.config.CronExpression} for {typeof(TQuery).Name}");
        }

        /// <summary>
        /// The actual workload and implementation for the IQuery
        /// </summary>
        /// <param name="cancellationToken">cancellation token for termination, use consistently in all downstream async operations</param>
        /// <returns>async void</returns>
        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"{this.GetType().Name} started");

            TModel queryResult = await query.Execute(cancellationToken);

            await publisher.Publish(queryResult, cancellationToken);

            Logger.LogInformation($"{queryResult}");
        }
    }
}
