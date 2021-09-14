using System;
using StackExchange.Redis;

namespace RedLock.Repositories.Redis
{
    public interface IRedisRepository
    {
        public void Add(string queue, string data);

        public string Get(string queue);

        public Lazy<ConnectionMultiplexer> GetConnection();
    }
}