using System;
using StackExchange.Redis;

namespace RedLock.Providers
{
    public interface IRedisProvider
    {
        public Lazy<ConnectionMultiplexer> CreateConnection();
        public void CloseConnection(Lazy<ConnectionMultiplexer> oldConnection);
        public IDatabase GetDatabase();
    }
}