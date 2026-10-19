using EHisse.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EHisse.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _tagTokens = new();

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null,
        string? tag = null)
    {
        if (_memoryCache.TryGetValue(key, out T? cachedValue) && cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();

        var entryOptions = new MemoryCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }
        if (slidingExpiration.HasValue)
        {
            entryOptions.SetSlidingExpiration(slidingExpiration.Value);
        }

        if (!string.IsNullOrEmpty(tag))
        {
            var cts = _tagTokens.GetOrAdd(tag, _ => new CancellationTokenSource());
            entryOptions.AddExpirationToken(new CancellationChangeToken(cts.Token));
        }

        _memoryCache.Set(key, value, entryOptions);
        return value;
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(
        string key,
        T value,
        TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null,
        string? tag = null)
    {
        var entryOptions = new MemoryCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            entryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
        }
        if (slidingExpiration.HasValue)
        {
            entryOptions.SetSlidingExpiration(slidingExpiration.Value);
        }

        if (!string.IsNullOrEmpty(tag))
        {
            var cts = _tagTokens.GetOrAdd(tag, _ => new CancellationTokenSource());
            entryOptions.AddExpirationToken(new CancellationChangeToken(cts.Token));
        }

        _memoryCache.Set(key, value, entryOptions);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public void RemoveByTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return;

        if (_tagTokens.TryRemove(tag, out var cts))
        {
            try
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }
            finally
            {
                cts.Dispose();
            }
        }
    }

    public void RemoveByTags(params string[] tags)
    {
        if (tags == null) return;
        foreach (var tag in tags)
        {
            RemoveByTag(tag);
        }
    }
}
