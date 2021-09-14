using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedLock.Repositories.Cache;
using RedLock.Repositories.Redis;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace RedLock.Services
{
    public class Runner : IHostedService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly RedLockFactory _redLockFactory;
        private readonly ILogger<Runner> _logger;
        private const string Resource = "redis-lock-queue";
        private readonly TimeSpan _expiry = TimeSpan.FromSeconds(30);
        private const string Key = "Entry";
        
        public Runner(IRedisRepository redisRepository, ILogger<Runner> logger, ICacheRepository cacheRepository)
        {
            _redisRepository = redisRepository;
            _logger = logger;
            _cacheRepository = cacheRepository;

            var multiplexers = new List<RedLockMultiplexer> {_redisRepository.GetConnection().Value};
            _redLockFactory = RedLockFactory.Create(multiplexers);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var task = Task.Run(async () => { await RedLock(); },
                        cancellationToken
                    );

                    var task1 = Task.Run(async () => { await RedLock(); },
                        cancellationToken
                    );

                    _redLockFactory.Dispose();
                    
                    Task.WaitAll(task, task1);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private async Task RedLock()
        {
            await using (var redLock = await _redLockFactory.CreateLockAsync(Resource, _expiry))
            {
                if (redLock.IsAcquired)
                {
                    Thread.Sleep(1000);
                    var data = _redisRepository.Get(Resource);
                    if (!string.IsNullOrEmpty(data))
                    {
                        _cacheRepository.Add(Key, Convert.ToInt32(data));
                    }
                    
                    return;
                }
            }

            if (_cacheRepository.TryGet(Key, out var res))
            {
                _cacheRepository.Remove(Key);
                _logger.LogInformation(res.ToString());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}