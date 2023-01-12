using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WatchDog.Common;
using WatchDog.Config;
using WatchDog.Model;
using WatchDog.Queries;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddSingleton<SomeQuery>()
            .AddSingleton<SomeOtherQuery>()
            .Configure<SomeQueryConfig>(context.Configuration.GetSection(typeof(SomeQueryConfig).Name))
            .Configure<SomeOtherQueryConfig>(context.Configuration.GetSection(typeof(SomeOtherQueryConfig).Name))
            .AddScheduledWatchdog<SomeQuery,BaseModel>(context.Configuration)
            .AddScheduledWatchdog<SomeOtherQuery, ExtendedModel>(context.Configuration);
    }).Build();

await host.RunAsync();