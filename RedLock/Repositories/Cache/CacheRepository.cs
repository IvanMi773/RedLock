using Microsoft.Extensions.Caching.Memory;

namespace RedLock.Repositories.Cache
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IMemoryCache _cache;

        public CacheRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Add(string key, int data)
        {
            _cache.Set(key, data);   
        }

        public bool TryGet(string key, out int data)
        {
            return _cache.TryGetValue(key, out data);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}