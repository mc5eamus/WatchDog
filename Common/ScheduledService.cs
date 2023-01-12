using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchDog.Config;

namespace WatchDog.Common
{
    public abstract class ScheduledService : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IScheduleConfig config;
        private readonly CronExpression cronExpression;
        private IOptions<ScheduleConfig<object>> config1;

        public ScheduledService(ILogger logger, IOptions<IScheduleConfig> config)
        {
            this.logger = logger;
            this.config = config.Value;
            this.cronExpression = CronExpression.Parse(config.Value.CronExpression);
        }

        protected ILogger Logger => logger;

        protected override async Task ExecuteAsync(CancellationToken ctoken)
        {
            while (!ctoken.IsCancellationRequested)
            {
                await WaitForNextSchedule(ctoken);

                Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await DoWork(ctoken);
            }
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);

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
