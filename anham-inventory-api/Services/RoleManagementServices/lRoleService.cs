using anham_inventory_api.Dtos.RoleDtos;
using anham_inventory_api.Models.UserManagemnt;
using Microsoft.AspNetCore.Identity;

namespace anham_inventory_api.Services.RoleManagementServices
{
    public interface IRoleService
    {
        Task<string> CreateRole(CreateRoleDto role, string createdBy);
        Task<IdentityResult> AssignRoleToUser(List<string> roleIds,ApplicationUser applicationUser);
        Task<ApplicationRole> FindRoleNameById(string roleId);
        List<string> GetUserRoleIds(string id);
        Task<string> DeActivate(string id);
        Task<string> Activate(string id);
        List<string> GetUserRole(string id);
    }
}
