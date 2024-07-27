using anham_inventory_api.Dtos.AuthDtos;
using anham_inventory_api.Dtos.RoleDtos;
using anham_inventory_api.Helpers.JwtConfigurations;
using anham_inventory_api.Services.AuthenticationServices;
using anham_inventory_api.Services.RoleManagementServices;
using anham_inventory_api.Services.UserManagementServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace anham_inventory_api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _AuthService;
        private readonly IUserManagementService _userManagementService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IRoleService _roleService;
        public AuthenticationController(IConfiguration configuration, IAuthService authService, IUserManagementService userManagementService, IJwtAuthManager jwtAuthManager, IRoleService roleService) 
        { 
            _configuration = configuration;
            _AuthService = authService;
            _userManagementService = userManagementService;
            _jwtAuthManager = jwtAuthManager;   
            _roleService = roleService;
        }

        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Validation Error" });
                }
                var result = await _AuthService.Login(dto);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return Unauthorized(new { Message = "Your Account is locked, try again after 15 minutes or contact to administration for further details." });
                    }
                    return Unauthorized(new { Message = "Invalid Login Attempt" });
                }
                var loggedInUser = _AuthService.GetUserByEmail(dto.Email);
                if (loggedInUser.IsDeleted == true)
                {
                    return Unauthorized(new { Message = "User with the given email has been Deleted" });
                }

                List<string> roles = _roleService.GetUserRole(loggedInUser.Id);
                List<string> roleIds = _roleService.GetUserRoleIds(loggedInUser.Id);
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Email,loggedInUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString()),
                    new Claim("userName", loggedInUser.UserName),
                    new Claim("fullName", loggedInUser.FullName),
                    new Claim("picture", loggedInUser.UserImage),
                    new Claim(ClaimTypes.Role, roles[0])
                };

                var jwtResult = _jwtAuthManager.GenerateTokens(loggedInUser.UserName, claims, DateTime.Now);

                return Ok(new LoginResult
                {
                    UserName = loggedInUser.UserName,
                    Email = loggedInUser.Email,
                    UserId = loggedInUser.Id,
                    FullName = loggedInUser.FullName,
                    Picture = loggedInUser.UserImage,
                    Roles = roles,
                    RoleIds = roleIds,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                string response = await _AuthService.Register(registerDto, "");
                if (response == "ALREADY_EXIST")
                {
                    return StatusCode(403, new { message = "User with " + registerDto.Email + " Email Already Exists" });
                }
                if (response == "NOT_REGISTERED")
                {
                    return StatusCode(400, new { message = "User Not Added Please Try Again" });
                }
                if (response == "NOT_ADDED_IN_ROLE")
                {
                    return StatusCode(201, new { message = "User Not Added In Selected Roles, Please Try Add Manually" });
                }
                return StatusCode(201, new { message = "User Registered Successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [Authorize]
        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto roles)
        {
            try
            {
                if (roles == null) return StatusCode(403, new { message = "Invalid Params Provided" });
                string response = await _roleService.CreateRole(roles, GetLoggedUserId());
                if (response == "ADDED_NEW_ROLES")
                {
                    return StatusCode(201, new { message = "New Role Created Successfully" });
                }
                return StatusCode(200, new { message = response + ": Already Added. Others Added Successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetLoggedUserId()
        {
            if (!User.Identity.IsAuthenticated)
                throw new AuthenticationException();

            var claims = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claims == null) return "";
            string userId = claims.Value;
            return userId;
        }
    }
}
