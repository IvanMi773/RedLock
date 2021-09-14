using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RedLock.Repositories;
using RedLock.Repositories.Redis;

namespace RedLock.Services
{
    public class RedisSenderService : IHostedService
    {
        private readonly IRedisRepository _redisRepository;

        public RedisSenderService(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(5000);
                    _redisRepository.Add("redis-lock-queue", new Random().Next(100).ToString());
                }
            }, cancellationToken);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}