using System.Collections.Generic;
using Domain.Interfaces;

namespace Application
{
    // naive implementation - not thread safe either
    public class CacheService : ICacheService
    {
        private static Dictionary<string, object> cache = new Dictionary<string, object>();

        public object Get(string cacheKey)
        {
            cache.TryGetValue(cacheKey, out var value);
            return value;
        }

        public bool Set(string cacheKey, object value)
        {
            return cache.TryAdd(cacheKey, value);
        }

        public void Remove(string cacheKey)
        {
            if (cache.ContainsKey(cacheKey))
            {
                cache.Remove(cacheKey);
            }
        }
    }
}
