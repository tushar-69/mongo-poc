using MongoPOC.Controllers;
using MongoPOC.Models;
using MongoPOC.Models.DTOs;
using MongoPOC.Services;

namespace MongoPOC.Test.Controllers
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly IFixture _fixture;
        private readonly Mock<IProductService> _mockProductService;
        private readonly List<ProductDTO> _products;

        public ProductControllerTests()
        {
            _fixture = new Fixture();
            _mockProductService = new Mock<IProductService>();
            _fixture.Register(() => _mockProductService.Object);
            _products = _fixture.CreateMany<ProductDTO>().ToList();
            _controller = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public async void Get_Products_ReturnsProducts()
        {
            _mockProductService.Setup(m => m.GetProductsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_products);

            var result = await _controller.Get(It.IsAny<int>(), It.IsAny<int>());

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<List<ProductDTO>>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().Equal(_products);
        }

        [Fact]
        public async void Get_ProductById_ReturnsProducts()
        {
            var product = _products.FirstOrDefault();
            var productID = _products.FirstOrDefault().Id;
            _mockProductService.Setup(m => m.GetProductByIdAsync(productID)).ReturnsAsync(product);

            var result = await _controller.GetById(productID);

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<ProductDTO>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async void Add_Product_ReturnsCreated()
        {
            Product product = _fixture.Create<Product>();
            var productID = product.Id;
            _mockProductService.Setup(m => m.CreateProductAsync(product)).ReturnsAsync(productID);

            var result = await _controller.Post(product);

            var objectResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var actualValue = objectResult.Value.Should().BeOfType<Product>().Subject;
            actualValue.Should().NotBeNull();
            actualValue.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async void Update_Product_ReturnsNoContent()
        {
            ProductDTO productDTO = _products.FirstOrDefault();
            Product product = new Product() { Id = productDTO.Id, Name = productDTO.Name, Category = productDTO.Category, Price = productDTO.Price };
            _mockProductService.Setup(m => m.UpdateProductAsync(product)).Returns(Task.CompletedTask);

            var result = await _controller.Update(product);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async void Delete_Product_ReturnsNoContent()
        {
            var productID = _products.FirstOrDefault().Id;
            _mockProductService.Setup(m => m.DeleteAsync(productID)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(productID);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async void Add_InvalidName_ReturnsValidationError()
        {
            _controller.ModelState.AddModelError("Name", "The field Name must be a string or array type with a minimum length of '3'.");
            _fixture.Customize<Product>(x => x.With(x => x.Name, "AB"));
            var product = _fixture.Create<Product>();

            var result = await _controller.Post(product);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            ((ModelStateDictionary.ValueEnumerable)actualValue.Value).AsEnumerable().ToList()[0].Errors[0].ErrorMessage.Should().Be("The field Name must be a string or array type with a minimum length of '3'.");
        }

        [Fact]
        public async void Add_InvalidCategory_ReturnsValidationError()
        {
            _controller.ModelState.AddModelError("Category", "The field Category must be a string or array type with a minimum length of '3'.");
            _fixture.Customize<Product>(x => x.With(x => x.Category, "AB"));
            var product = _fixture.Create<Product>();

            var result = await _controller.Post(product);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            ((ModelStateDictionary.ValueEnumerable)actualValue.Value).AsEnumerable().ToList()[0].Errors[0].ErrorMessage.Should().Be("The field Category must be a string or array type with a minimum length of '3'.");
        }

        [Fact]
        public async void Add_InvalidPrice_ReturnsValidationError()
        {
            _controller.ModelState.AddModelError("Price", "The field Price must be a string or array type with a minimum length of '3'.");
            _fixture.Customize<Product>(x => x.With(x => x.Price, -1));
            var product = _fixture.Create<Product>();

            var result = await _controller.Post(product);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            ((ModelStateDictionary.ValueEnumerable)actualValue.Value).AsEnumerable().ToList()[0].Errors[0].ErrorMessage.Should().Be("The field Price must be a string or array type with a minimum length of '3'.");
        }

        [Fact]
        public async void Get_InvalidProduct_ReturnsNotFound()
        {
            _mockProductService.Setup(m => m.GetProductByIdAsync(It.IsAny<string>())).Throws(new InvalidOperationException("Product not found"));

            var result = await _controller.GetById(It.IsAny<string>());

            var actualValue = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            actualValue.Value.Should().Be("Product not found");
        }

        [Fact]
        public async void Update_InvalidProduct_ReturnsBadRequest()
        {
            var product = _fixture.Create<Product>();
            _mockProductService.Setup(m => m.UpdateProductAsync(product)).Throws(new InvalidOperationException("Failed to update product"));

            var result = await _controller.Update(product);

            var actualValue = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            actualValue.Value.Should().Be("Failed to update product");
        }

        [Fact]
        public async void Delete_InvalidProduct_ReturnsNotFound()
        {
            var productID = new Guid().ToString();
            _mockProductService.Setup(m => m.DeleteAsync(productID)).Throws(new InvalidOperationException("Failed to delete product"));

            var result = await _controller.Delete(productID);

            var actualValue = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            actualValue.Value.Should().Be("Failed to delete product");
        }
    }
}
