using anham_inventory_api.Models.UserManagemnt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace anham_inventory_api.Context.Mappings.UserManagement
{
    public class ApplicationUserRoleMap : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasKey(x => new { x.UserId, x.RoleId });
            builder.HasOne(x => x.ApplicationUser).WithMany(x => x.ApplicationUserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ApplicationRole).WithMany(x => x.ApplicationUserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        }      
    }
}
