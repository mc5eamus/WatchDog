namespace WatchDog.Config
{
    public interface IScheduleConfig
    {
        string CronExpression { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig
    {
        public string CronExpression { get; set; } = string.Empty;
    }
}
