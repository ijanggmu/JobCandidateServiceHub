using JobCandidate.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace JobCandidate.Infrastructure.Caching
{
    public class CacheRepository<T> : ICacheRepository<T> where T : class
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration;

        public CacheRepository(IMemoryCache cache, TimeSpan? cacheDuration = null)
        {
            _cache = cache;
            _cacheDuration = cacheDuration ?? TimeSpan.FromMinutes(5); // Default to 5 minutes
        }

        public T Get(string key)
        {
            _cache.TryGetValue(key, out T item);
            return item;
        }

        public void Set(string key, T item)
        {
            _cache.Set(key, item, _cacheDuration);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }

}
