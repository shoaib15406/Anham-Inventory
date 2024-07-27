using anham_inventory_api.Models.UserManagemnt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace anham_inventory_api.Context.Mappings.UserManagement
{
    public class ApplicationRoleMap : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(c => c.CreatedBy).HasColumnName("CreatedBy").IsRequired(false);
            builder.Property(c => c.UpdatedBy).HasColumnName("UpdatedBy").IsRequired(false);
            builder.Property(c => c.CreatedOn).HasColumnName("CreatedOn");
            builder.Property(c => c.UpdatedOn).HasColumnName("UpdatedOn");
            builder.Property(c => c.IsActive).HasColumnName("IsActive");
            builder.Property(c => c.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
