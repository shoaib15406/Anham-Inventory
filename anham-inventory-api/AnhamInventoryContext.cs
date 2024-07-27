using anham_inventory_api.Context.Mappings.UserManagement;
using anham_inventory_api.Mappings.UserManagementMapping;
using anham_inventory_api.Models.UserManagemnt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace anham_inventory_api
{
    public class AnhamInventoryContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public AnhamInventoryContext(DbContextOptions<AnhamInventoryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // user managment mapping integrations starts
            builder.ApplyConfiguration<ApplicationUser>(new ApplicationUserMap());
            builder.ApplyConfiguration<ApplicationRole>(new ApplicationRoleMap());
            builder.ApplyConfiguration<ApplicationUserRole>(new ApplicationUserRoleMap());
            // user managment mapping integrations ends

        }

        #region user management
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        #endregion


    }
}
