using ProductService.Application.Dtos;
using SharedContracts.Dtos;

namespace ProductService.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductRequest createProductRequest);
        Task<ProductDto?> UpdateProductAsync(string id, UpdateProductRequest updateProductRequest);
        Task<bool> DeleteProductAsync(string id);
        Task<ProductDto?> GetProductAsync(string id);
        Task<List<ProductDto>> GetProductsAsync();
    }
}