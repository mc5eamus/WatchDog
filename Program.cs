using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly.Registry;
using WatchDog.Common;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Model;
using WatchDog.Publishers;
using WatchDog.Queries;
using WatchDog.Resiliency;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .Configure<SomeQueryConfig>(context.Configuration.GetSection(typeof(SomeQueryConfig).Name))
            .Configure<SomeOtherQueryConfig>(context.Configuration.GetSection(typeof(SomeOtherQueryConfig).Name))
            .AddResiliencyPolicyRegistry()
            .AddSingleton<SomeQuery>()
            .AddSingleton<SomeOtherQuery>()
            .AddSingleton<SomePublisher>()
            .AddSingleton<SomeOtherPublisher>()
            .AddSingleton<IPublisher, CompositePublisher>()
            .AddScheduledWatchdog<SomeQuery, BaseModel>(context.Configuration)
            .AddScheduledWatchdog<SomeOtherQuery, ExtendedModel>(context.Configuration);
    }).Build();

await host.RunAsync();