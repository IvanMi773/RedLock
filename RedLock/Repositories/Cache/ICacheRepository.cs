namespace RedLock.Repositories.Cache
{
    public interface ICacheRepository
    {
        public void Add(string key, int data);
        public bool TryGet(string key, out int data);
        public void Remove(string key);
    }
}