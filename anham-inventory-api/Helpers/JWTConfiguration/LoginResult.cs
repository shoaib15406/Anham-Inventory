using System.Text.Json.Serialization;

namespace anham_inventory_api.Helpers.JwtConfigurations
{
    public class LoginResult
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;  

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new List<string>();

        [JsonPropertyName("roleIds")]
        public List<string> RoleIds { get; set; } = new List<string>();

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
