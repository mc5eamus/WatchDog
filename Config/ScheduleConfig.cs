namespace WatchDog.Config
{
    /// <summary>
    /// Defines a contract for job scheduling config
    /// </summary>
    public interface IScheduleConfig
    {
        /// <summary>
        /// Defines how frequently to run the job in the CRON format
        /// </summary>
        string CronExpression { get; set; }
    }

    /// <summary>
    /// Implements <see cref="IScheduleConfig">the contract</see> and provides a specific implementation for typed scheduling configs 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScheduleConfig<TWorker> : IScheduleConfig
    {
        public string CronExpression { get; set; } = string.Empty;
        
        // for a multiconfig scenario, identifies configs to run on a given schedule
        public IEnumerable<string> Configs { get; set; } = Enumerable.Empty<string>();
    }

}
