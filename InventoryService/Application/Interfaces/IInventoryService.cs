using InventoryService.Application.Dtos;
using SharedContracts.Dtos;

namespace InventoryService.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto request);
        Task<InventoryDto?> UpdateInventoryAsync(string id, UpdateInventoryDto request);
        Task<bool> DeleteInventoryAsync(string id);
        Task<InventoryDto?> GetInventoryByIdAsync(string id);
        Task<List<InventoryDto>> GetAllInventoryAsync();
    }
}