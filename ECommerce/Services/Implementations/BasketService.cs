using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Basket;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(
            IBasketRepository basketRepository,
            IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDto> GetBasketAsync(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                throw new EmptyBasketException(basketId);

            var mappedBasket = _mapper.Map<BasketDTO>(basket);
            return ResponseDto.Success(200, "Basket retrieved successfully", mappedBasket);
        }

        public async Task<ResponseDto> CreateBasketAsync(BasketDTO basketDto)
        {
            var basket = _mapper.Map<Basket>(basketDto);

            basket.Id = null;

            var createdBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket);

            if (createdBasket is null)
                return ResponseDto.Failure(500, "Failed to create basket");

            var mappedBasket = _mapper.Map<BasketDTO>(createdBasket);
            return ResponseDto.Success(201, "Basket created successfully", mappedBasket);
        }

        public async Task<ResponseDto> AddItemToBasketAsync(string basketId, BasketItem item)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return ResponseDto.Failure(404, "Basket not found");

            // Check if item already exists in basket
            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem is not null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                basket.Items.Add(item);
            }

            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return ResponseDto.Failure(500, "Failed to add item to basket");

            var mappedBasket = _mapper.Map<BasketDTO>(updatedBasket);
            return ResponseDto.Success(200, "Item added to basket successfully", mappedBasket);
        }

        public async Task<ResponseDto> UpdateItemQuantityAsync(string basketId, int productId, int quantity)
        {
            if (quantity <= 0)
                return ResponseDto.Failure(400, "Quantity must be greater than zero");

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return ResponseDto.Failure(404, "Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item is null)
                return ResponseDto.Failure(404, "Item not found in basket");

            item.Quantity = quantity;
            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return ResponseDto.Failure(500, "Failed to update item quantity");

            var mappedBasket = _mapper.Map<BasketDTO>(updatedBasket);
            return ResponseDto.Success(200, "Item quantity updated successfully", mappedBasket);
        }

        public async Task<ResponseDto> RemoveItemFromBasketAsync(string basketId, int productId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return ResponseDto.Failure(404, "Basket not found");

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item is null)
                return ResponseDto.Failure(404, "Item not found in basket");

            basket.Items.Remove(item);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return ResponseDto.Failure(500, "Failed to remove item from basket");

            var mappedBasket = _mapper.Map<BasketDTO>(updatedBasket);
            return ResponseDto.Success(200, "Item removed from basket successfully", mappedBasket);
        }

        public async Task<ResponseDto> ClearBasketAsync(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return ResponseDto.Failure(404, "Basket not found");

            basket.Items.Clear();
            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            if (updatedBasket is null)
                return ResponseDto.Failure(500, "Failed to clear basket");

            var mappedBasket = _mapper.Map<BasketDTO>(updatedBasket);
            return ResponseDto.Success(200, "Basket cleared successfully", mappedBasket);
        }

        public async Task<ResponseDto> GetBasketTotalAsync(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return ResponseDto.Failure(404, "Basket not found");

            var total = basket.Items.Sum(item => item.Price * item.Quantity);

            return ResponseDto.Success(200, "Basket total calculated successfully", new { Total = total });
        }
    }
}