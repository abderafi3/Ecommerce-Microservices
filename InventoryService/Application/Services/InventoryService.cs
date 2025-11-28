using InventoryService.Application.Dtos;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using SharedContracts.Dtos;

namespace InventoryService.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;

        public InventoryService(IInventoryRepository repo) => _repo = repo;

        public async Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto request)
        {
            var inventory = new Inventory
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                LastUpdated = DateTime.UtcNow
            };

            await _repo.AddAsync(inventory);

            return new InventoryDto(inventory.ProductId, inventory.Quantity);
        }

        public async Task<bool> DeleteInventoryAsync(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<List<InventoryDto>> GetAllInventoryAsync()
        {
            var inventories = await _repo.GetAllAsync();
            return inventories.Select(i => new InventoryDto(i.ProductId, i.Quantity)).ToList();
        }

        public async Task<InventoryDto?> GetInventoryByIdAsync(string id)
        {
            var inventory = await _repo.GetByIdAsync(id);
            return inventory == null ? null : new InventoryDto(inventory.ProductId, inventory.Quantity);
        }

        public async Task<InventoryDto?> UpdateInventoryAsync(string id, UpdateInventoryDto request)
        {
            var inventory = await _repo.GetByIdAsync(id);
            if (inventory == null) return null;

            inventory.ProductId = request.ProductId;
            inventory.Quantity = request.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;

            await _repo.UpdateAsync(inventory);

            return new InventoryDto(inventory.ProductId, inventory.Quantity);
        }
    }
}