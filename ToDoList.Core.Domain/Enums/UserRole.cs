namespace ToDoList.Core.Domain.Enums
{
    /// <summary>
    /// Defines the roles available in the system
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Regular user with basic permissions
        /// </summary>
        User = 1,

        /// <summary>
        /// Administrator with elevated permissions
        /// </summary>
        Admin = 2,

        /// <summary>
        /// System administrator with full permissions
        /// </summary>
        SuperAdmin = 3
    }
}
