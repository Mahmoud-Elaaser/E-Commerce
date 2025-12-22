using ECommerce.DTOs;
using ECommerce.DTOs.Basket;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("{basketId}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBasket(string basketId)
        {
            var response = await _basketService.GetBasketAsync(basketId);
            return StatusCode(response.Status, response);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrGetBasket([FromForm] BasketDTO basketDTO)
        {
            var response = await _basketService.CreateBasketAsync(basketDTO);
            return StatusCode(response.Status, response);
        }

        [HttpPost("{basketId}/items")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItemToBasket(string basketId, [FromBody] BasketItem item)
        {
            if (item == null)
            {
                return BadRequest(ResponseDto.Failure(400, "Item cannot be null"));
            }

            var response = await _basketService.AddItemToBasketAsync(basketId, item);
            return StatusCode(response.Status, response);
        }

        [HttpPut("{basketId}/items/{productId}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItemQuantity(
            string basketId,
            int productId,
            [FromBody] UpdateQuantityRequestDto request)
        {
            if (request == null)
            {
                return BadRequest(ResponseDto.Failure(400, "Request body cannot be null"));
            }

            var response = await _basketService.UpdateItemQuantityAsync(basketId, productId, request.Quantity);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{basketId}/items/{productId}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItemFromBasket(string basketId, int productId)
        {
            var response = await _basketService.RemoveItemFromBasketAsync(basketId, productId);
            return StatusCode(response.Status, response);
        }


        [HttpDelete("{basketId}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearBasket(string basketId)
        {
            var response = await _basketService.ClearBasketAsync(basketId);
            return StatusCode(response.Status, response);
        }


        [HttpGet("{basketId}/total")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBasketTotal(string basketId)
        {
            var response = await _basketService.GetBasketTotalAsync(basketId);
            return StatusCode(response.Status, response);
        }

        [HttpGet("my-basket")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyBasket()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ResponseDto.Failure(401, "User not authenticated"));
            }

            var response = await _basketService.GetBasketAsync(userId);
            return StatusCode(response.Status, response);
        }


        [HttpPost("my-basket/items")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddItemToMyBasket([FromBody] BasketItem item)
        {
            if (item == null)
            {
                return BadRequest(ResponseDto.Failure(400, "Item cannot be null"));
            }

            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ResponseDto.Failure(401, "User not authenticated"));
            }

            var response = await _basketService.AddItemToBasketAsync(userId, item);
            return StatusCode(response.Status, response);
        }
    }

}