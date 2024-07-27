namespace anham_inventory_api.Dtos.AdminDtos
{
    public class createRoomDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Price { get; set; }
        public int RoomType { get; set; }
    }
}
