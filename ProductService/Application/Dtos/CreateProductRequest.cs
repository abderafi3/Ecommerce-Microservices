using System.ComponentModel.DataAnnotations;

namespace ProductService.Application.Dtos
{
    public record CreateProductRequest(
        [Required] string ProductName,
        [Required] string ProductDescription,
        [Range(0.01, double.MaxValue)] decimal ProductPrice);

    public record UpdateProductRequest(
        [Required] string ProductName,
        [Required] string ProductDescription,
        [Range(0.01, double.MaxValue)] decimal ProductPrice);
}