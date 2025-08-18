using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using SharedData.DTOs.E_CommerceDTOs;
using SharedData.Wrapper;
using System.Collections.Generic;
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Location))
            {
                return BadRequest("Location is required.");
            }

            var order = await serviceManager.orderService.CreateOrderAsync(dto.Location);
            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully."));
        }


        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var orders = await serviceManager.orderService.GetUserOrdersAsync();
            if (orders == null || orders.Count == 0)
                return NotFound("No orders found for this user.");

            return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully."));
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Order ID must be greater than zero.");

            var order = await serviceManager.orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound($"Order with ID {orderId} not found.");

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully."));
        }
    }
}
