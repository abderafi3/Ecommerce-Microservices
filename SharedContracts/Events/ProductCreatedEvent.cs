using System;

namespace SharedContracts.Events
{
    public record ProductCreatedEvent(string ProductId, string ProductName, string Description, decimal Price, DateTime CreatedAt);
}
