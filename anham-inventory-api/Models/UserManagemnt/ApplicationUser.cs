using Microsoft.AspNetCore.Identity;

namespace anham_inventory_api.Models.UserManagemnt
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public Nullable<DateTime> CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string UserImage { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public ICollection<ApplicationUserRole>? ApplicationUserRoles { get; set; }
    }
}
