using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces;
using ToDoList.Core.Domain.Shared;
using ToDoList.Infrastructure.Persistence.Context;
using ToDoList.Infrastructure.Persistence.Repositories.Common;

namespace ToDoList.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for User entity operations
    /// </summary>
    public class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        private readonly ToDoListContext _context;

        public UserRepository(ToDoListContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Set<User>()
                .AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Set<User>()
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Override methods to handle the repository pattern correctly
        public async Task<OperationResult<User>> AddAsync(User entity)
        {
            return await base.AddAsync(entity, CancellationToken.None);
        }

        public async Task<OperationResult<User>> UpdateAsync(User entity)
        {
            return await base.UpdateAsync(entity, CancellationToken.None);
        }

        public async Task<OperationResult<User>> DeleteAsync(Guid id)
        {
            var entity = await _context.Set<User>().FindAsync(id);
            if (entity == null)
                return OperationResult<User>.Failure(new List<string> { "User not found" });

            return await base.DeleteAsync(entity, CancellationToken.None);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Set<User>().FindAsync(id);
        }
    }
}
