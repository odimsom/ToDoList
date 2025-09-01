using Microsoft.AspNetCore.Authorization;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Attributes
{
    /// <summary>
    /// Authorization attribute for role-based access control
    /// </summary>
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
