using ProductService.Application.Dtos;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using SharedContracts.Dtos;
using SharedContracts.Events;
using StackExchange.Redis;
using System.Text.Json;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, IConnectionMultiplexer redis, ILogger<ProductService> logger)
        {
            _repository = repository;
            _redis = redis;
            _logger = logger;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = request.ProductName,
                ProductDescription = request.ProductDescription,
                ProductPrice = request.ProductPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(product);

            var evt = new ProductCreatedEvent(product.ProductId, product.ProductName, product.ProductDescription, product.ProductPrice, product.CreatedAt);
            var json = JsonSerializer.Serialize(evt);
            var db = _redis.GetDatabase();
            await db.StreamAddAsync("product-events", new NameValueEntry[] { new("eventType", nameof(ProductCreatedEvent)), new("data", json) });

            _logger.LogInformation("Product created and event published: {ProductId}", product.ProductId);

            return new ProductDto(product.ProductId, product.ProductName, product.ProductDescription, product.ProductPrice);
        }

        public async Task<ProductDto?> GetProductAsync(string productId)
        {
            var product = await _repository.GetByIdAsync(productId);
            return product == null ? null : new ProductDto(product.ProductId, product.ProductName, product.ProductDescription, product.ProductPrice);
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductDto(p.ProductId, p.ProductName, p.ProductDescription, p.ProductPrice)).ToList();
        }

        public async Task<ProductDto?> UpdateProductAsync(string id, UpdateProductRequest request)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            product.ProductName = request.ProductName;
            product.ProductDescription = request.ProductDescription;
            product.ProductPrice = request.ProductPrice;

            await _repository.UpdateAsync(product);

            return new ProductDto(product.ProductId, product.ProductName, product.ProductDescription, product.ProductPrice);
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}