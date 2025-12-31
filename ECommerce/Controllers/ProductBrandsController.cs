using ECommerce.DTOs;
using ECommerce.DTOs.Product;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductBrandsController : ControllerBase
    {
        private readonly IProductBrandService _brandService;

        public ProductBrandsController(IProductBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBrands()
        {
            var result = await _brandService.GetAllBrandsAsync();
            return StatusCode(result.Status, result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            return StatusCode(result.Status, result);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBrand([FromBody] CreateOrUpdateBrandDto dto)
        {
            var result = await _brandService.CreateBrandAsync(dto);

            if (result.Status == 201 && result.Model != null)
            {
                return CreatedAtAction(
                    nameof(GetBrandById),
                    new { id = ((ProductBrandDto)result.Model)?.Id },
                    result);
            }

            return StatusCode(result.Status, result);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] CreateOrUpdateBrandDto dto)
        {
            var result = await _brandService.UpdateBrandAsync(id, dto);
            return StatusCode(result.Status, result);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            return StatusCode(result.Status, result);
        }
    }
}