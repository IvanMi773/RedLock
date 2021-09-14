using System;
using RedLock.Providers;
using StackExchange.Redis;

namespace RedLock.Repositories.Redis
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redis;
        private readonly Lazy<ConnectionMultiplexer> _connection;

        public RedisRepository(RedisProvider redisProvider)
        {
            _redis = redisProvider.GetDatabase();
            _connection = redisProvider.CreateConnection();
        }

        public void Add(string queue, string data)
        {
            _redis.ListLeftPush(queue, data);
        }

        public string Get(string queue)
        {
            return _redis.ListRightPop(queue);
        }

        public Lazy<ConnectionMultiplexer> GetConnection()
        {
            return _connection;
        }
    }
}