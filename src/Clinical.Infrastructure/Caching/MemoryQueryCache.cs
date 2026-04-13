using Clinical.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Clinical.Infrastructure.Caching;

public sealed class MemoryQueryCache(IMemoryCache memoryCache) : IQueryCache
{
    private readonly HashSet<string> _keys = [];
    private readonly object _sync = new();

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(key, out T? cached) && cached is not null)
        {
            return cached;
        }

        var result = await factory(cancellationToken);
        memoryCache.Set(key, result, expiration);
        lock (_sync)
        {
            _keys.Add(key);
        }
        return result;
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
        lock (_sync)
        {
            _keys.Remove(key);
        }
    }

    public void RemoveByPrefix(string prefix)
    {
        string[] keysToRemove;
        lock (_sync)
        {
            keysToRemove = _keys.Where(x => x.StartsWith(prefix, StringComparison.Ordinal)).ToArray();
        }

        foreach (var key in keysToRemove)
        {
            Remove(key);
        }
    }
}
