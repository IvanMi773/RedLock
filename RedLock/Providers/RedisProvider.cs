using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RedLock.Providers
{
    public class RedisProvider : IRedisProvider
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private ConnectionMultiplexer Connection => _lazyConnection.Value;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RedisProvider> _logger;

        public RedisProvider(IConfiguration configuration, ILogger<RedisProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _lazyConnection = CreateConnection();
        }

        public Lazy<ConnectionMultiplexer> CreateConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                var cacheConnection = _configuration["CacheConnection"];
                return ConnectionMultiplexer.Connect(cacheConnection);
            });
        }

        public void CloseConnection(Lazy<ConnectionMultiplexer> oldConnection)
        {
            if (oldConnection == null)
                return;

            try
            {
                oldConnection.Value.Close();
            }
            catch (Exception)
            {
                _logger.LogError("Error while closing connection");
            }
        }

        public IDatabase GetDatabase()
        {
            return Connection.GetDatabase();
        }
    }
}