namespace Market.Application.Models.Responses;

public class BrandDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
