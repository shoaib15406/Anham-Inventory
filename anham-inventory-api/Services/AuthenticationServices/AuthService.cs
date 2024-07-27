using Microsoft.AspNetCore.Identity;
using anham_inventory_api.Dtos.AuthDtos;
using Microsoft.EntityFrameworkCore;
using System.Text;
using anham_inventory_api.Models.UserManagemnt;
using anham_inventory_api.Services.RoleManagementServices;
using anham_inventory_api.Services.UserManagementServices;

namespace anham_inventory_api.Services.AuthenticationServices
{
    public class AuthService: IAuthService
    {
        private readonly AnhamInventoryContext _context;
        private readonly IUserManagementService _userMangementService;
        private readonly IRoleService _rolesMangement;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthService(IUserManagementService userManagementService,
            AnhamInventoryContext context, IRoleService rolesMangement, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userMangementService = userManagementService;
            _rolesMangement = rolesMangement;
            _userManager = userManager;
        }
        public async Task<SignInResult> Login(LoginDto dto)
        {
            try
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                SignInResult result = await ValidateUser(dto.Email, dto.Password);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SignInResult> ValidateUser(string email, string password)
        {
         
            var result = await _userMangementService.AuthenticateUser(email, password);
            return result;
        }
        
        public ApplicationUser GetUserByEmail(string email)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(c => c.UserName == email);
                if (user != null) return user;
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> Register(RegisterDto dto, string createdBy)
        {
            try
            {
                var userExists = await _userMangementService.userExists(dto.Email);
                if (userExists)
                {
                    return "ALREADY_EXIST";
                }
                var roleId = await _context.ApplicationRoles.FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Role.ToLower());
                List<string> roleIds = new List<string>();
                roleIds.Add(roleId!.Id);
                var User = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName= dto.FirstName,
                    LastName= dto.LastName,
                    EmailConfirmed = true,
                    UserImage = dto.UserImage,
                    PhoneNumber = dto.PhoneNumber,
                    CreatedBy= createdBy,
                    FullName = dto.FirstName + " " + dto.LastName,
                    CreatedOn = DateTime.Now,
                    LockoutEnabled = true,
                    Status = "ACTIVE",
                };

                string randomPassword = GeneratePassword();
                var result = await _userMangementService.RegisterNewUser(User, dto.Password);
                if (!result.Succeeded)
                {
                    return "NOT_REGISTERED";
                }
                var response = await _rolesMangement.AssignRoleToUser(roleIds, User);
                if (response.Succeeded == false)
                {
                    if (!response.Succeeded)
                    {
                        return "NOT_ADDED_IN_ROLE";
                    }
                }
                return "User Registered Successfully";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApplicationUser> GetUser(string Id, bool isCurrentUser)
        {
            try
            {
                var quary = _context.Users.AsQueryable();
                if (isCurrentUser)
                    quary = quary.IgnoreQueryFilters();
                var user = await quary.FirstOrDefaultAsync(u => u.Id == Id);
                if (user == null) return null;
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveAll()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                throw;
            }           
        }

        public IList<string> CatchIdentityErrors(IdentityResult identityResult)
        {
            var errors = new List<string>();
            foreach (var error in identityResult.Errors)
            {
                errors.Add(error.Description);
            }
            return errors;
        }

        public string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }
    }
}