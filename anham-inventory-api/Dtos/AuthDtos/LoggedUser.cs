
namespace anham_inventory_api.Dtos.AuthDtos
{
    public class LoggedUser
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
        public string[]? Roles { get; set; }
    }
}
