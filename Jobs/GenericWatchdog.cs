using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDog.Common;
using WatchDog.Config;
using WatchDog.Contract;

namespace WatchDog.Jobs
{
    public class GenericWatchdog<T, M> : ScheduledService
        where T : IQuery<M>
        where M : class
    {
        private readonly T query;
        private readonly ILogger logger;
        private readonly IScheduleConfig config;

        public GenericWatchdog(T query, ILogger<T> logger, IOptions<ScheduleConfig<T>> config) : base (logger, config)
        {
            this.query = query;
            this.logger = logger;
            this.config = config.Value;
            logger.LogInformation($"{this.config.CronExpression} for {typeof(T).Name}");
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"{this.GetType().Name} started");

            M queryResult = await query.Execute(cancellationToken);

            Logger.LogInformation($"{queryResult.ToString()}");
        }
    }
}
