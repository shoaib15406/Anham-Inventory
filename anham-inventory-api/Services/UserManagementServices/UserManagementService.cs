using anham_inventory_api.Services.RoleManagementServices;
using anham_inventory_api.Models.UserManagemnt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace anham_inventory_api.Services.UserManagementServices
{
    public class UserManagementService : IUserManagementService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AnhamInventoryContext _context;
        private readonly IRoleService _roleService;

        public UserManagementService(UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager, AnhamInventoryContext context, IRoleService roleService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleService = roleService;
        }

        public async Task<bool> userExists(string Email)
        {
            var userExists = await _userManager.FindByEmailAsync(Email);
            if (userExists == null)
            {
                return false;
            }
            return true;
        }

        public async Task<IdentityResult> RegisterNewUser(ApplicationUser applicationUser, string Password)
        {
            var result = await _userManager.CreateAsync(applicationUser, Password);
            if (!result.Succeeded)
            {
                return result;
            }
            return IdentityResult.Success;
        }

        public async Task<SignInResult> AuthenticateUser(string Email, string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(Email, Password, false, true);
            return result;
        }

        public async Task<ApplicationUser> FindUserByEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null) return null;
            return user;
        }

        public async Task<string> GetUsersEmails(string userIds)
        {
            try
            {
                var users = await _context.ApplicationUsers.ToListAsync();
                var emails = users.Where(x => userIds.Split(",").Contains(x.Id)).Select(x => x.Email).ToList();
                return String.Join(",", emails);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
   
}
