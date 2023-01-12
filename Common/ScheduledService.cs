using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDog.Config;

namespace WatchDog.Common
{
    /// <summary>
    /// Base class for scheduled job execution
    /// </summary>
    public abstract class ScheduledService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IScheduleConfig config;
        private readonly CronExpression cronExpression;

        /// <summary>
        /// Supposed to be created by DI
        /// </summary>
        /// <param name="logger">Logger (we're keeping it neutral for now, the inheriting class can request a specific one)</param>
        /// <param name="config">Config for scheduling (don't confuse with Query settings like connection strings</param>
        public ScheduledService(ILogger logger, IOptions<IScheduleConfig> config)
        {
            this.logger = logger;
            this.config = config.Value;
            this.cronExpression = CronExpression.Parse(config.Value.CronExpression);
        }

        /// <summary>
        /// Make the logger available to the inheriting classes in a safe way
        /// </summary>
        protected ILogger Logger => logger;

        /// <summary>
        /// Overrides the BackgroundService exec
        /// </summary>
        /// <param name="ctoken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken ctoken)
        {
            while (!ctoken.IsCancellationRequested)
            {
                await WaitForNextSchedule(ctoken);

                Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await DoWork(ctoken);
            }
        }

        /// <summary>
        /// Supposed to be implemented by inheriting classes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task DoWork(CancellationToken cancellationToken);

        /// <summary>
        /// Central piece of the scheduler: calculated a timespan to wait for the next scheduled execution
        /// </summary>
        /// <param name="ctoken"></param>
        /// <returns></returns>
        private async Task WaitForNextSchedule(CancellationToken ctoken)
        {
            var currentUtcTime = DateTimeOffset.UtcNow.UtcDateTime;
            var occurenceTime = cronExpression.GetNextOccurrence(currentUtcTime);
            var delay = occurenceTime.GetValueOrDefault().Subtract(currentUtcTime);

            Logger.LogInformation("The run is delayed for {delay}. Current time: {time}", delay, DateTimeOffset.Now);

            await Task.Delay(delay, ctoken);
        }
    }
}
