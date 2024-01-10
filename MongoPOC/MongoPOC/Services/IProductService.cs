using MongoPOC.Models;
using MongoPOC.Models.DTOs;

namespace MongoPOC.Services
{
    public interface IProductService
    {
        Task<string> CreateProductAsync(Product product);

        Task<List<ProductDTO>> GetProductsAsync(int page, int size);

        Task<ProductDTO> GetProductByIdAsync(string id);

        Task UpdateProductAsync(Product product);

        Task DeleteAsync(string id);
    }
}
