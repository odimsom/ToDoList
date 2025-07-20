using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Core.Domain.Entities;

namespace ToDoList.Infrastructure.Persistence.Configurations
{
    public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("TaskItems");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(e => e.StatusTask)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.DueDate);
        }
    }
}
