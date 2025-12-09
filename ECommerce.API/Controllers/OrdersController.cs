using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IBasketService _basketService;

        public OrdersController(IOrderService orderService, IBasketService basketService)
        {
            _orderService = orderService;
            _basketService = basketService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order is null)
                return NotFound();

            // Verify user owns the order
            //var userId = GetUserId();
            //if (order.UserId != userId && !User.IsInRole("Admin"))
            //    return Forbid();

            return Ok(order.Value); ///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(Guid userId)
        {
            // Verify user can access these orders
            var currentUserId = GetUserId();
            if (userId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var userId = GetUserId();
                var basket = await _basketService.GetBasketAsync(dto.BasketId);

                if (basket is null)
                    return BadRequest("Basket is empty");

                var order = await _orderService.CreateOrderFromBasketAsync(
                    userId,
                    basket,
                    dto.ShippingAddress,
                    dto.BillingAddress
                );

                // Clear the basket after successful order
                await _basketService.DeleteBasketAsync(dto.BasketId);

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Order>> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var changedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
                var order = await _orderService.UpdateOrderStatusAsync(id, dto.Status, changedBy);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<Order>> CancelOrder(Guid id, [FromBody] CancelOrderDto dto)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order is null)
                    return NotFound();

                //var userId = GetUserId();
                //if (order.UserId != userId && !User.IsInRole("Admin"))
                //    return Forbid();

                var cancelledOrder = await _orderService.CancelOrderAsync(id, dto.Reason);
                return Ok(cancelledOrder);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? Guid.Empty.ToString());
        }
    }
}
