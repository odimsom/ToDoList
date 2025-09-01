using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ToDoList.Core.Application.Interfaces;

namespace ToDoList.Core.Application.Services
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CachingService> _logger;
        private readonly HashSet<string> _cacheKeys = new();
        private readonly object _lockObject = new();

        public CachingService(IMemoryCache memoryCache, ILogger<CachingService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var value = _memoryCache.Get<T>(key);
                if (value != null)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                }
                return await Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration.Value;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                }

                options.RegisterPostEvictionCallback((k, v, reason, state) =>
                {
                    lock (_lockObject)
                    {
                        _cacheKeys.Remove(k.ToString()!);
                    }
                });

                _memoryCache.Set(key, value, options);

                lock (_lockObject)
                {
                    _cacheKeys.Add(key);
                }

                _logger.LogDebug("Cached value for key: {Key}", key);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _memoryCache.Remove(key);
                lock (_lockObject)
                {
                    _cacheKeys.Remove(key);
                }
                _logger.LogDebug("Removed cache for key: {Key}", key);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                List<string> keysToRemove;
                lock (_lockObject)
                {
                    keysToRemove = _cacheKeys.Where(k => k.Contains(pattern, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key, cancellationToken);
                }

                _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
            }
        }

        public string GenerateQueryKey<T>(T query) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(query, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });
                var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json)));
                return $"{typeof(T).Name}_{hash}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cache key for query type: {Type}", typeof(T).Name);
                return $"{typeof(T).Name}_{Guid.NewGuid()}";
            }
        }
    }
}