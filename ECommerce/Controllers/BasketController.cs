using AutoMapper;
using ECommerce.DTOs;
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
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Basket>> GetBasket(string basketId)
        {
            var response = await _basketRepository.GetBasketAsync(basketId);
            return response is null ? new Basket(basketId) : response;
        }

        [HttpPost]
        public async Task<ActionResult<Basket>> UpdateBasket(CustomerBasketDto basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, Basket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpdatedBasket is null) return BadRequest("There is a problem Wih Your Basket :(");
            return Ok(CreatedOrUpdatedBasket);
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }
        [HttpPost("create")]
        public async Task<ActionResult<BasketResponseDto>> CreateOrUpdateBasket([FromBody] CustomerBasketDto basketDto)
        {
            var basket = _mapper.Map<Basket>(basketDto);

            // Ensure a new ID is generated for create operations
            basket.Id = Guid.NewGuid().ToString();

            var savedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket);

            if (savedBasket == null)
            {
                return StatusCode(500, "Failed to save basket");
            }


            var response = _mapper.Map<BasketResponseDto>(savedBasket);
            return Ok(response);
        }


    }

}