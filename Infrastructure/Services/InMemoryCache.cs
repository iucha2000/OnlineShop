using Application.Services;
using Application.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class InMemoryCache : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly CachingOptions _options;

        public InMemoryCache(IMemoryCache cache, IOptions<CachingOptions> options)
        {
            _cache = cache;
            _options = options.Value;
        }

        public T GetData<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void RemoveData(string key)
        {
            _cache.Remove(key);
        }

        public bool SetData<T>(string key, T value, MemoryCacheEntryOptions options = null)
        {
            try
            {
                if(options == null)
                {
                    options = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(_options.AbsoluteExpiration),
                        SlidingExpiration = TimeSpan.FromSeconds(_options.SlidingExpiration),
                        Priority = CacheItemPriority.Normal,
                        Size = _options.Size,
                    };
                }

                _cache.Set(key, value, options);
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
