using System.ComponentModel.DataAnnotations;

namespace InventoryService.Application.Dtos
{
    public record CreateInventoryDto(
        [Required] string ProductId,
        [Range(0, int.MaxValue)] int Quantity);

    public record UpdateInventoryDto(
        [Required] string ProductId,
        [Range(0, int.MaxValue)] int Quantity);
}