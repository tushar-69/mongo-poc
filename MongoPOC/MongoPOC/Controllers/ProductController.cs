using Microsoft.AspNetCore.Mvc;
using MongoPOC.Models;
using MongoPOC.Models.DTOs;
using MongoPOC.Services;

namespace MongoPOC.Controllers
{
    [Controller]
    [Route("/Product")]
    public class ProductController : ControllerBase
    {
        public readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery]int page = 1, [FromQuery]int size = 10)
        {
            var response = await _productService.GetProductsAsync(page, size);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var response = await _productService.GetProductByIdAsync(id);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var ID = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { Id = ID }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            try
            {
                await _productService.UpdateProductAsync(product);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
