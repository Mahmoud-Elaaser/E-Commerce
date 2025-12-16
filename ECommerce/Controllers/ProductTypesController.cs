using ECommerce.DTOs.Product;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly IProductTypeService _typeService;

        public ProductTypesController(IProductTypeService typeService)
        {
            _typeService = typeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypes()
        {
            var result = await _typeService.GetAllTypesAsync();
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeById(int id)
        {
            var result = await _typeService.GetTypeByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateType([FromBody] CreateOrUpdateTypeDto dto)
        {
            var result = await _typeService.CreateTypeAsync(dto);

            if (result.Status == 201 && result.Model != null)
            {
                return CreatedAtAction(
                    nameof(GetTypeById),
                    new { id = ((ProductTypeDto)result.Model)?.Id },
                    result);
            }

            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateType(int id, [FromBody] CreateOrUpdateTypeDto dto)
        {
            var result = await _typeService.UpdateTypeAsync(id, dto);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteType(int id)
        {
            var result = await _typeService.DeleteTypeAsync(id);
            return StatusCode(result.Status, result);
        }
    }
}
