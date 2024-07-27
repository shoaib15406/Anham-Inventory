using anham_inventory_api.Dtos.RoleDtos;
using anham_inventory_api.Models.UserManagemnt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace anham_inventory_api.Services.RoleManagementServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AnhamInventoryContext _context;

        public RoleService(RoleManager<ApplicationRole> roleManager, AnhamInventoryContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IdentityResult> AssignRoleToUser(List<string> roleIds, ApplicationUser applicationUser)
        {
            try
            {
                List<ApplicationUserRole> userRoles = new List<ApplicationUserRole>();
                foreach (var roleId in roleIds)
                {
                    userRoles.Add(new ApplicationUserRole
                    {
                        UserId = applicationUser.Id,
                        RoleId = roleId
                    });
                }
                if (userRoles.Count > 0)
                {
                   await _context.UserRoles.AddRangeAsync(userRoles);
                    await _context.SaveChangesAsync();
                }
                return IdentityResult.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetUserRole(string id)
        {
            try
            {
                List<string> roles = new List<string>();
                var userRoles = _context.ApplicationRoles.ToList();
                var usersRoleIds = _context.UserRoles.Where(c => c.UserId == id).ToList();
                foreach (var item in usersRoleIds)
                {
                    var role = userRoles.FirstOrDefault(c => c.Id == item.RoleId);
                    if (role != null)
                    {
                        string userRoleName = role.Name;
                        roles.Add(userRoleName);
                    }
                }
                return roles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> CreateRole(CreateRoleDto role, string createdBy)
        {
            try
            {
                var listRoles = new List<ApplicationRole>();
                var alreadyExistNames = new List<string>();
                bool roleExist = await _context.ApplicationRoles.AnyAsync(x => x.Name.ToLower().Trim() == role.RoleName.ToLower().Trim());
                if (!roleExist)
                {
                    var roleNew = new ApplicationRole
                    {
                        Name = role.RoleName,
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        Status = "ACTIVE"
                    };
                    await _context.ApplicationRoles.AddAsync(roleNew);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    alreadyExistNames.Add(role.RoleName);
                }
                if (alreadyExistNames.Count > 0)
                {
                    return String.Concat(alreadyExistNames.ToArray());
                }
                return "ADDED_NEW_ROLES";
            }
            catch (Exception)
            {
                throw;
            }           
        }

        public async Task<ApplicationRole> FindRoleNameById(string roleId)
        {
            try
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
                if (role != null) return role;
                return null;
            }
            catch (Exception)
            {
                throw;
            }           
        }

        public async Task<string> Activate(string id)
        {
            try
            {
                var found = await _context.ApplicationRoles.FirstOrDefaultAsync(x => x.Id == id);
                if (found == null)
                {
                    return "NOT_FOUND";
                }
                found.Status = "ACTIVE";
                _context.ApplicationRoles.Update(found);
                await _context.SaveChangesAsync();
                return "Role has been Activated successfully";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeActivate(string id)
        {
            try
            {
                var found = await _context.ApplicationRoles.FirstOrDefaultAsync(x => x.Id == id);
                if (found == null)
                {
                    return "NOT_FOUND";
                }
                found.Status = "INACTIVE";
                _context.ApplicationRoles.Update(found);
                await _context.SaveChangesAsync();
                return "Role has been deactivated successfully";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetUserRoleIds(string id)
        {
            try
            {
                List<string> roles = new List<string>();
                var userRoles = _context.ApplicationRoles.ToList();
                var usersRoleIds = _context.UserRoles.Where(c => c.UserId == id).ToList();
                foreach (var item in usersRoleIds)
                {
                    var role = userRoles.FirstOrDefault(c => c.Id == item.RoleId);
                    if (role != null)
                    {
                        string userRoleId = role.Id;
                        roles.Add(userRoleId);
                    }
                }
                return roles;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckDuplicatRole(string name, string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    return await _context.ApplicationRoles.
                        AnyAsync(x => x.Name.ToLower().Trim() == name.ToLower().Trim()
                                      && x.Id != id);

                }
                return await _context.ApplicationRoles.
                        AnyAsync(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
