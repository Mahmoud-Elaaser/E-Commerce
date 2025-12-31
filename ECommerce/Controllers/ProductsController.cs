using ECommerce.DTOs;
using ECommerce.DTOs.Pagination;
using ECommerce.DTOs.Product;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductPaginationParams paginationParams)
        {
            var result = await _productService.GetAllProductsAsync(paginationParams);
            return StatusCode(result.Status, result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductById(
            int id,
            [FromQuery] bool includeBrand = false,
            [FromQuery] bool includeType = false)
        {
            var result = await _productService.GetProductByIdAsync(id, includeBrand, includeType);
            return StatusCode(result.Status, result);
        }


        [HttpGet("search")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string searchTerm,
            [FromQuery] PaginationParams paginationParams)
        {
            var result = await _productService.SearchProductsAsync(searchTerm, paginationParams);
            return StatusCode(result.Status, result);
        }


        [HttpGet("brand/{brandId}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductsByBrand(
            int brandId,
            [FromQuery] PaginationParams paginationParams)
        {
            var result = await _productService.GetProductsByBrandAsync(brandId, paginationParams);
            return StatusCode(result.Status, result);
        }


        [HttpGet("type/{typeId}")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductsByType(
            int typeId,
            [FromQuery] PaginationParams paginationParams)
        {
            var result = await _productService.GetProductsByTypeAsync(typeId, paginationParams);
            return StatusCode(result.Status, result);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromForm] AddOrUpdateProductDto addProductDto)
        {
            var result = await _productService.AddProductAsync(addProductDto);

            if (result.Status == 201 && result.Model != null)
            {
                var productId = (result.Model as dynamic)?.Id;
                if (productId != null)
                {
                    return CreatedAtAction(
                        nameof(GetProductById),
                        new { id = productId },
                        result);
                }
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
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromForm] AddOrUpdateProductDto updateProductDto)
        {
            var result = await _productService.UpdateProductAsync(id, updateProductDto);
            return StatusCode(result.Status, result);
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return StatusCode(result.Status, result);
        }


    }
}