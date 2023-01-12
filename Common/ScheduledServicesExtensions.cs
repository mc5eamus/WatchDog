using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Jobs;

namespace WatchDog.Common
{
    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddScheduledWatchdog<TQuery,TModel>(this IServiceCollection services, 
                Action<IScheduleConfig> options)
            where TQuery : IQuery<TModel>
            where TModel : class
        {
            services
                .Configure<ScheduleConfig<TQuery>>(options)
                .AddHostedService<GenericWatchdog<TQuery, TModel>>();
            return services;
        }

        public static IServiceCollection AddScheduledWatchdog<TQuery, TModel>(this IServiceCollection services, 
                IConfiguration config)
            where TQuery : IQuery<TModel>
            where TModel : class
        {
            services
                .Configure<ScheduleConfig<TQuery>>(config.GetSection(typeof(TQuery).Name))
                .AddHostedService<GenericWatchdog<TQuery, TModel>>();
            return services;
        }
    }
}
