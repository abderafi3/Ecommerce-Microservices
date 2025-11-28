using Microsoft.EntityFrameworkCore;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Persistence;

namespace InventoryService.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _db;

        public InventoryRepository(InventoryDbContext db) => _db = db;

        public async Task AddAsync(Inventory inventory)
        {
            await _db.Inventories.AddAsync(inventory);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var inventory = await _db.Inventories.FindAsync(id);
            if (inventory == null) return false;

            _db.Inventories.Remove(inventory);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _db.Inventories.ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(string id)
        {
            return await _db.Inventories.FindAsync(id);
        }

        public async Task UpdateAsync(Inventory inventory)
        {
            _db.Inventories.Update(inventory);
            await _db.SaveChangesAsync();
        }
    }
}