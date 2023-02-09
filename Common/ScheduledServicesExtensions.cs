using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        /// Configures a watchdog with a query (including its returned model), taking the schedule from the configuration
        /// </summary>
        /// <typeparam name="TQuery">Type of the query to execute on a schedule</typeparam>
        /// <typeparam name="TModel">Model returned by the query</typeparam>
        /// <param name="services">services collection</param>
        /// <param name="config">IConfiguration to take the schedule config from</param>
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


        /// <summary>
        /// Initiates multiconfig watchdogs for a set of schedules defined in configuration
        /// Naming conventions: 
        /// - Schedules are picked from an item named by DataSource type
        /// - Worker configurations are picked from an item named by Config type
        /// </summary>
        /// <typeparam name="TDataSource">Type of the data source to execute on a schedule</typeparam>
        /// <typeparam name="TConfig">Type of configuration TDataSource is consuming</typeparam>
        /// <typeparam name="TModel">Model returned by the query</typeparam>
        /// <param name="services">services collection</param>
        /// <param name="config">IConfiguration to take the schedule and worker config from</param>
        /// <returns></returns>
        public static IServiceCollection AddScheduledMulticonfigWatchdog<TDataSource, TConfig, TModel>(
            this IServiceCollection services,
                IConfiguration config)
            where TDataSource : IDataSource<TModel, TConfig>
            where TConfig : class
            where TModel : class
        {
            var scheduleConfig = new DataSourceScheduleConfig<ScheduleConfig<TDataSource>>();
            var workerConfig = new DataSourceConfig<TConfig>();
            config.GetSection(typeof(TDataSource).Name).Bind(scheduleConfig);
            config.GetSection(typeof(TConfig).Name).Bind(workerConfig);

            // for every schedule config in a dictionary, one instance of MulticonfigWatchdog
            // will be created and started
            foreach (var v in scheduleConfig)
            {
                // Schedule config defines when and which configs to run the worker, referring them by key
                var configs = workerConfig.Where(_ => v.Value.Configs.Contains(_.Key)).Select(_ => _.Value);
                var configOptions = Options.Create(configs);

                var scheduleOptions = Options.Create(v.Value);

                services
                    .AddSingleton<IHostedService>(p => {
                        var logger = p.GetService<ILogger<TDataSource>>();
                        logger?.LogInformation($"{v.Key} config with option(s) {string.Join(',', v.Value.Configs)}");

                        // We're using ActivatorUtilities.CreateInstance to supply
                        // manual parameters to DI (Schedule and Workload Config),
                        // others (DataSource, Publisher, Logger) will be taken care of automatically.
                        return ActivatorUtilities.CreateInstance<MulticonfigWatchdog<TDataSource, TConfig, TModel>>(p, 
                            new object[] { configOptions, scheduleOptions }); });
            }

            return services;
        }
    }
}
