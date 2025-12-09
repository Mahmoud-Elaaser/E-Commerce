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
    [Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetLowStock()
        {
            var inventory = await _inventoryService.GetLowStockAsync();
            return Ok(inventory);
        }

        [HttpPost("adjust")]
        public async Task<ActionResult> AdjustInventory([FromBody] AdjustInventoryDto dto)
        {
            try
            {
                var changedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
                await _inventoryService.AdjustInventoryAsync(
                    dto.ProductId,
                    dto.ProductVariantId,
                    dto.Quantity,
                    dto.Reason,
                    dto.Notes,
                    changedBy
                );
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
