using Microsoft.AspNetCore.Identity;

namespace anham_inventory_api.Models.UserManagemnt
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUser? ApplicationUser { get; set; }
        public ApplicationRole? ApplicationRole { get; set; }
    }
}
