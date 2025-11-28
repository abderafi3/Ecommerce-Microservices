namespace InventoryService.Domain.Entities
{
    public class Inventory
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}