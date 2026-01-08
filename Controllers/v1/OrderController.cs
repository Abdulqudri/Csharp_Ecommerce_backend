using Ecommerce.API.Constants;
using Ecommerce.API.Controllers;
using Ecommerce.API.Dtos.Requests.Orders;
using Ecommerce.API.Dtos.Responses.Orders;
using Ecommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (UserId == null)
                return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(UserId, page, pageSize);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            if (UserId == null)
                return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, UserId);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (UserId == null)
                return Unauthorized();

            var order = await _orderService.CreateOrderAsync(request, UserId);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = RoleConstants.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request.Status, UserId!);
            if (!result)
                return NotFound();

            return Ok(new { message = "Order status updated successfully" });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (UserId == null)
                return Unauthorized();

            var result = await _orderService.CancelOrderAsync(id, UserId);
            if (!result)
                return NotFound();

            return Ok(new { message = "Order cancelled successfully" });
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetOrderSummary()
        {
            if (UserId == null)
                return Unauthorized();

            var summary = await _orderService.GetOrderSummaryAsync(UserId);
            return Ok(summary);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }
    }
}