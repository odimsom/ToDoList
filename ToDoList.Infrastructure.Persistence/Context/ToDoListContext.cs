using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ToDoList.Core.Domain.Common;
using ToDoList.Core.Domain.Entities;

namespace ToDoList.Infrastructure.Persistence.Context
{
    public class ToDoListContext : DbContext
    {
        public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options)
        {
            
        }
        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
                    var notDeleted = Expression.Not(property);
                    var lambda = Expression.Lambda(notDeleted, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "DefaultAppUser";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = "DefaultAppUser";
                        break;
                    case EntityState.Deleted:
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = DateTime.Now;
                        entry.Entity.DeletedBy = "DefaultAppUser";
                        entry.State = EntityState.Modified;
                        break;
                }
            }
            return base.SaveChanges();
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "DefaultAppUser";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = "DefaultAppUser";
                        break;
                    case EntityState.Deleted:
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = DateTime.Now;
                        entry.Entity.DeletedBy = "DefaultAppUser";
                        entry.State = EntityState.Modified;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
