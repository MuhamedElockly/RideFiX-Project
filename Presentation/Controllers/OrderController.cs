using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using SharedData.DTOs.E_CommerceDTOs;
using SharedData.Wrapper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public OrderController(IServiceManager _serviceManager)
        {
            serviceManager = _serviceManager;
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue("userId"); 
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var orders = await serviceManager.orderService.GetUserOrdersAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound(ApiResponse<List<OrderDto>>.FailResponse("No orders found."));

            return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully."));
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userId = User.FindFirstValue("userId"); 
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var order = await serviceManager.orderService.GetOrderByIdAsync(orderId);

      

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully."));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            if (string.IsNullOrWhiteSpace(dto.Location))
                return BadRequest("Location is required.");

            var order = await serviceManager.orderService.CreateOrderAsync(dto.Location, userId);

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully."));
        }
    }

   
}
