namespace anham_inventory_api.Dtos.AdminDtos
{
    public class GetCustomerRequestDto
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string CheckIn { get; set; } = string.Empty;
        public string CheckOut { get; set; } = string.Empty;
        public string BookingDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
    }
}
