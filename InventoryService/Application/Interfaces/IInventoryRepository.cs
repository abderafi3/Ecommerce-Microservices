using InventoryService.Domain.Entities;

namespace InventoryService.Application.Interfaces
{
    public interface IInventoryRepository
    {
        Task AddAsync(Inventory inventory);
        Task<bool> DeleteAsync(string id);
        Task<Inventory?> GetByIdAsync(string id);
        Task UpdateAsync(Inventory inventory);
        Task<List<Inventory>> GetAllAsync();
    }
}