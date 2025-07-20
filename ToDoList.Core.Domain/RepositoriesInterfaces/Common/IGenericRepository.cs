using ToDoList.Core.Domain.Shared;

namespace ToDoList.Core.Domain.RepositoriesInterfaces.Common
{
    public interface IGenericRepository<TEntity, TType> 
        where TEntity : class
    {
        Task<OperationResult<TEntity>> GetByIdAsync(TType Id, CancellationToken cancellationToken);
        Task<OperationResult<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken);
        Task<OperationResult<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<OperationResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<OperationResult<TEntity>> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
