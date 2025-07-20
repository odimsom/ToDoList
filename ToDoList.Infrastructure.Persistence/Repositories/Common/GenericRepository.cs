using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Domain.RepositoriesInterfaces.Common;
using ToDoList.Core.Domain.Shared;

namespace ToDoList.Infrastructure.Persistence.Repositories.Common
{
    public class GenericRepository<TEntity, TType> : IGenericRepository<TEntity, TType>
        where TEntity : class
    {
        private readonly DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _context.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return OperationResult<TEntity>.Success(entity);
        }

        public async Task<OperationResult<TEntity>> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return OperationResult<TEntity>.Success(entity);
        }

        public async Task<OperationResult<IEnumerable<TEntity>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.Set<TEntity>().ToListAsync();
            return OperationResult<IEnumerable<TEntity>>.Success(entities);
        }

        public async Task<OperationResult<TEntity>> GetByIdAsync(TType Id, CancellationToken cancellationToken)
        {
            var entity = await _context.FindAsync<TEntity>(Id);
            return OperationResult<TEntity>.Success(entity!);
        }

        public async Task<OperationResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Update<TEntity>(entity);
            await _context.SaveChangesAsync();
            return OperationResult<TEntity>.Success(entity);
        }
    }
}
