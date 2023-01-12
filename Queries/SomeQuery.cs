﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatchDog.Config;
using WatchDog.Contract;
using WatchDog.Model;

namespace WatchDog.Queries
{
    public class SomeQuery : IQuery<BaseModel>
    {
        ILogger logger;
        SomeQueryConfig config;

        public SomeQuery(ILogger<SomeQuery> logger, IOptions<SomeQueryConfig> config) { 
            this.logger = logger;
            this.config = config.Value;
            logger.LogInformation($"...using {this.config.SomeServiceConnectionString}");
        }

        public async Task<BaseModel> Execute(CancellationToken ctoken)
        {
            int delay = 1 + Random.Shared.Next(10);
            logger.LogInformation($"{this.GetType().Name} is running for {delay}s");
            await Task.Delay(1000 * delay, ctoken);
            return await Task.FromResult(new BaseModel() { Id = 1 });
        }
    }
}
