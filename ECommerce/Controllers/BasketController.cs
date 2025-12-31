using AutoMapper;
using ECommerce.DTOs.Basket;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet("{basketId}")]
        public async Task<ActionResult<Basket>> GetBasket(string basketId)
        {
            var response = await _basketRepository.GetBasketAsync(basketId);
            return response is null ? new Basket(basketId) : response;
        }

        [HttpPost]
        public async Task<ActionResult<Basket>> UpdateBasket([FromBody] CustomerBasketDto basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, Basket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpdatedBasket is null) return BadRequest("There is a problem Wih Your Basket");
            return Ok(CreatedOrUpdatedBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var result = await _basketRepository.DeleteBasketAsync(id);
            if (result) return Ok(new { message = "Basket deleted successfully" });
            return StatusCode(500, new { message = "Failed to delete baskets" });
        }

        [HttpDelete("clear-all")]
        public async Task<IActionResult> ClearAllBaskets()
        {
            var result = await _basketRepository.ClearAllBasketsAsync();

            if (result)
                return Ok(new { message = "All baskets cleared successfully" });

            return StatusCode(500, new { message = "Failed to clear baskets" });
        }


    }

}