namespace Clinical.Application.Abstractions.Caching;

public interface IQueryCache
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken);

    void Remove(string key);
    void RemoveByPrefix(string prefix);
}
