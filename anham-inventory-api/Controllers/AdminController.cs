using anham_inventory_api.Dtos.AdminDtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anham_inventory_api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public AdminController() 
        {
        }

        #region confirm booking

        [Authorize(Roles = "admin")]
        [HttpPost("confirmOrder")]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmBookingDto confirmBooking)
        {
            try
            {
                if(confirmBooking == null) { return BadRequest(new { message = "Invalid data" }); }
                string response = string.Empty;
                if(response != "BOOKING_CONFIRMED" && response != "REQUEST_REJECTED") { return BadRequest(new { message = "Booking not confirmed, please try again later" }); }
                if (response == "REQUEST_REJECTED") return Ok(new { message = "Request Rejected Successfully" });
                return Ok(new { message = "Booking confirmed" });
            }
            catch (NullReferenceException)
            {
                return BadRequest(new { message = "Null value not allowed" });
            }
            catch (InvalidCastException)
            {
                return BadRequest(new { message = "Invalid casting" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region customer requests
        [Authorize(Roles = "admin")]
        [HttpGet("getCustomerRequest")]
        public async Task<IActionResult> GetCustomerRequest()
        {
            try
            {
                var customerRequest = new { };
                return Ok(new { data = customerRequest });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
    }
}
