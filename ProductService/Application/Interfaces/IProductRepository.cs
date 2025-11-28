using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task<bool> DeleteAsync(string id);
        Task<Product?> GetByIdAsync(string id);
        Task<List<Product>> GetAllAsync();
    }
}