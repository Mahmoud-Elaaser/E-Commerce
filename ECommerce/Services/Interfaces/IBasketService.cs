using ECommerce.DTOs;
using ECommerce.DTOs.Basket;
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IBasketService
    {
        Task<ResponseDto> GetBasketAsync(string basketId);

        Task<ResponseDto> CreateBasketAsync(BasketDTO basketDto);

        Task<ResponseDto> AddItemToBasketAsync(string basketId, BasketItem item);

        Task<ResponseDto> UpdateItemQuantityAsync(string basketId, int productId, int quantity);
        Task<ResponseDto> RemoveItemFromBasketAsync(string basketId, int productId);


        Task<ResponseDto> ClearBasketAsync(string basketId);

        Task<ResponseDto> GetBasketTotalAsync(string basketId);
    }
}