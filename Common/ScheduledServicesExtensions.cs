using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Jobs;

namespace WatchDog.Common
{
    /// <summary>
    /// Extensions methods as syntactic support for elegantly configure Watchdog jobs for dependency injection
    /// </summary>
    public static class ScheduledServiceExtensions
    {
        /// <summary>
        /// Configures a watchdog with a query (including its returned model), allowing a delegate to return the scheduling config
        /// </summary>
        /// <typeparam name="TQuery">Type of the query to execute on a schedule</typeparam>
        /// <typeparam name="TModel">Model returned by the query</typeparam>
        /// <param name="services">services collection</param>
        /// <param name="options">A delegate returning the configuration value (helpful for manual configs and tests) </param>
        /// <returns></returns>
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

        /// <summary>
        /// Configures a watchdog with a query (including its returned model), taking the scheduling config from the config
        /// </summary>
        /// <typeparam name="TQuery">Type of the query to execute on a schedule</typeparam>
        /// <typeparam name="TModel">Model returned by the query</typeparam>
        /// <param name="services">services collection</param>
        /// <param name="config">IConfiguration to take the sceduling config from</param>
        /// <returns></returns>
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
