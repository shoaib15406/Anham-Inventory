using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anham_inventory_api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public CustomerController()
        {
        }

        [Authorize]
        [HttpGet("getCustomerBookingData/{customerId}")]
        public async Task<IActionResult> GetCustomerBookingData(string customerId)
        {
            try
            {
                if (String.IsNullOrEmpty(customerId)) return BadRequest(new { message = "Invalid data" });
                var customerData = new { id = 0, name = "" };
                return Ok(new { data = customerData });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("bookRoom")]
        public async Task<IActionResult> BookRoom()
        {
            try
            {
                string response = string.Empty;
                if (response != "ROOM_BOOKED") return BadRequest(new { message = "Room not booked, please try again later" });
                return Ok(new { message = "Room booking request send" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
