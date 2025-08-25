using ToDoList.Core.Application.Wrapper;

namespace ToDoList.Core.Application.Interfaces
{
    public interface IIdempotencyService
    {
        Task<ResponseService<T>?> GetIdempotentResponseAsync<T>(string idempotencyKey, CancellationToken cancellationToken = default);
        Task StoreIdempotentResponseAsync<T>(string idempotencyKey, ResponseService<T> response, CancellationToken cancellationToken = default);
        Task InvalidateIdempotentResponseAsync(string pattern, CancellationToken cancellationToken = default);
        string GenerateIdempotencyKey(string operation, object identifier);
        bool HasDataChanged<T>(T current, T previous) where T : class;
    }
}