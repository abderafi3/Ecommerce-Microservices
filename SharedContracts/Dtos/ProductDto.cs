using System;

namespace SharedContracts.Dtos
{
    public record ProductDto(string ProductId, string ProductName, string Description, decimal Price);
}
