using anham_inventory_api.Models.UserManagemnt;
using Microsoft.AspNetCore.Identity;

namespace anham_inventory_api.Context.DataSeeding
{
    public class DataSeeder
    {
        private readonly AnhamInventoryContext _dBContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataSeeder(AnhamInventoryContext dBContext, UserManager<ApplicationUser> userManager)
        {
            _dBContext = dBContext;
            _userManager = userManager;
        }

        public void Seed()
        {
            if (!_dBContext.ApplicationUsers.Any() && !_dBContext.ApplicationRoles.Any())
            {
                UserAndRoleSeeding();
            }
        }

        public void UserAndRoleSeeding()
        {
            try
            {
                var roles = RolesSeed();
                _dBContext.ApplicationRoles.AddRange(roles);
                _dBContext.SaveChanges();
                var user = new ApplicationUser
                {
                    FullName = "Super Admin",
                    FirstName = "Super",
                    LastName = "Admin",
                    IsDeleted = false,
                    CreatedOn = DateTime.Now,
                    UserImage = "",
                    Status = "ACTIVE",
                    UserName = "superadmin@hotel.com.sa",
                    Email = "superadmin@hotel.com.sa",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                };
                _userManager.CreateAsync(user, "anham@123").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, "admin").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicationRole> RolesSeed()
        {
            var roles = new List<ApplicationRole>()
            {
                new ApplicationRole()
                {
                    Name = "admin",
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Status = "ACTIVE",
                    NormalizedName = "ADMIN"
                },
                new ApplicationRole()
                {
                    Name = "customer",
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Status = "ACTIVE",
                    NormalizedName = "CUSTOMER"
                }
            };
            return roles;
        }
    }
}
