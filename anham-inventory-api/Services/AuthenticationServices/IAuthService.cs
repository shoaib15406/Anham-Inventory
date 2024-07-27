using anham_inventory_api.Dtos.AuthDtos;
using anham_inventory_api.Models.UserManagemnt;
using Microsoft.AspNetCore.Identity;

namespace anham_inventory_api.Services.AuthenticationServices
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDto dto, string createdBy);
        Task<SignInResult> Login(LoginDto model);
        Task<ApplicationUser> GetUser(string Id, bool isCurrentUser);
        ApplicationUser GetUserByEmail(string email);
        Task<bool> SaveAll();
        Task<SignInResult> ValidateUser(string email, string password);
        IList<string> CatchIdentityErrors(IdentityResult identityResult);
    }
}
