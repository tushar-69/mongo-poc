using MongoPOC.Models;
using MongoPOC.Models.DTOs;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AutoMapper;
using MongoDB.Bson;

namespace MongoPOC.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private IMapper _mapper;

        public ProductService(IOptions<MongoDBSettings> mongoDBSettings, IMapper mapper) 
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _productsCollection = database.GetCollection<Product>(mongoDBSettings.Value.CollectionName);
            _mapper = mapper;
        }

        public async Task<string> CreateProductAsync(Product product)
        {
            await _productsCollection.InsertOneAsync(product);
            return product.Id;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(Product => Product.Id, id);
            var response = await _productsCollection.DeleteOneAsync(filter);
            if (!response.IsAcknowledged) throw new InvalidOperationException("Failed to delete product");
            return;
        }

        public async Task<ProductDTO> GetProductByIdAsync(string id)
        {
            var filter = Builders<Product>.Filter.Eq(Product => Product.Id, id);
            var product = await _productsCollection.Find(filter).FirstOrDefaultAsync();
            if (product == null) throw new InvalidOperationException("Product not found");
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<List<ProductDTO>> GetProductsAsync(int page, int size)
        {
            int skip = (page - 1) * size;
            var products = await _productsCollection.Find(new BsonDocument()).Skip(skip).Limit(size).ToListAsync();
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task UpdateProductAsync(Product product)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(Product => Product.Id, product.Id);
            UpdateDefinition<Product> update = Builders<Product>.Update
                                            .Set(Product => Product.Name, product.Name)
                                            .Set(Product => Product.Category, product.Category)
                                            .Set(Product => Product.Price, product.Price)
                                            .Set(Product => Product.Reviews, product.Reviews);
            var response = await _productsCollection.UpdateOneAsync(filter, update);
            if (!response.IsAcknowledged) throw new InvalidOperationException("Failed to update product");
            return;
        }
    }
}
