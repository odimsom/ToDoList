using Microsoft.Extensions.Logging;
using System.Text.Json;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Wrapper;

namespace ToDoList.Core.Application.Services
{
    public class IdempotencyService : IIdempotencyService
    {
        private readonly ICachingService _cachingService;
        private readonly ILogger<IdempotencyService> _logger;
        private readonly TimeSpan _idempotencyExpiration = TimeSpan.FromHours(24);

        public IdempotencyService(ICachingService cachingService, ILogger<IdempotencyService> logger)
        {
            _cachingService = cachingService;
            _logger = logger;
        }

        public async Task<ResponseService<T>?> GetIdempotentResponseAsync<T>(string idempotencyKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"idempotency_{idempotencyKey}";
                var cachedResponse = await _cachingService.GetAsync<ResponseService<T>>(cacheKey, cancellationToken);

                if (cachedResponse != null)
                {
                    _logger.LogDebug("Idempotent response found for key: {Key}", idempotencyKey);
                }

                return cachedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving idempotent response for key: {Key}", idempotencyKey);
                return null;
            }
        }

        public async Task StoreIdempotentResponseAsync<T>(string idempotencyKey, ResponseService<T> response, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = $"idempotency_{idempotencyKey}";
                await _cachingService.SetAsync(cacheKey, response, _idempotencyExpiration, cancellationToken);

                _logger.LogDebug("Stored idempotent response for key: {Key}", idempotencyKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing idempotent response for key: {Key}", idempotencyKey);
            }
        }

        public async Task InvalidateIdempotentResponseAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cachingService.RemoveByPatternAsync($"idempotency_{pattern}", cancellationToken);
                _logger.LogDebug("Invalidated idempotent responses for pattern: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating idempotent responses for pattern: {Pattern}", pattern);
            }
        }

        public string GenerateIdempotencyKey(string operation, object identifier)
        {
            try
            {
                var combined = $"{operation}_{JsonSerializer.Serialize(identifier)}";
                var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(combined)));
                return hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating idempotency key for operation: {Operation}", operation);
                return $"{operation}_{Guid.NewGuid()}";
            }
        }

        public bool HasDataChanged<T>(T current, T previous) where T : class
        {
            try
            {
                if (current == null && previous == null) return false;
                if (current == null || previous == null) return true;

                var currentJson = JsonSerializer.Serialize(current, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var previousJson = JsonSerializer.Serialize(previous, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return !string.Equals(currentJson, previousJson, StringComparison.Ordinal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing data for type: {Type}", typeof(T).Name);
                return true; // Assume changed if comparison fails
            }
        }
    }
}