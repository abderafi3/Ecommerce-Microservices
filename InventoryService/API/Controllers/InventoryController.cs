using InventoryService.Application.Dtos;
using InventoryService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers
{
    [ApiController]
    [Route("api/inventories")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllInventoryAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var inventory = await _service.GetInventoryByIdAsync(id);
            return inventory == null ? NotFound() : Ok(inventory);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateInventoryDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inventory = await _service.CreateInventoryAsync(request);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = inventory.ProductId }, inventory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateInventoryDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateInventoryAsync(id, request);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var deleted = await _service.DeleteInventoryAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}