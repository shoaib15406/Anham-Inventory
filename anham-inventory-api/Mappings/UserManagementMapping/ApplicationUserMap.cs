using anham_inventory_api.Models.UserManagemnt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace anham_inventory_api.Mappings.UserManagementMapping
{
    public class ApplicationUserMap : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FullName).HasColumnName("FullName").HasMaxLength(200).IsRequired(true);
            builder.Property(x => x.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired(true);
            builder.Property(x => x.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired(true);
            builder.Property(x => x.UserImage).HasColumnName("UserImage").IsRequired(false);
            builder.Property(x => x.Status).HasColumnName("Status").HasMaxLength(20).IsRequired(false);

            builder.Property(c => c.CreatedBy).HasColumnName("CreatedBy").IsRequired(false);
            builder.Property(c => c.UpdatedBy).HasColumnName("UpdatedBy").IsRequired(false);
            builder.Property(c => c.CreatedOn).HasColumnName("CreatedOn");
            builder.Property(c => c.UpdatedOn).HasColumnName("UpdatedOn");
            builder.Property(c => c.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
