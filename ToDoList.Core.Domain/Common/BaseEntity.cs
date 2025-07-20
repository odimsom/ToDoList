namespace ToDoList.Core.Domain.Common
{
    /// <summary>
    /// Represents a base entity with a strongly-typed identifier.
    /// This generic base class provides a required <c>Id</c> property, allowing derived entities to specify the type of their unique identifier.
    /// </summary>
    /// <typeparam name="TType">The type of the entity's identifier (e.g., <c>int</c>, <c>Guid</c>, <c>string</c>).</typeparam>
    public class BaseEntity<TType> : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public required TType Id { get; set; }
    }
}
