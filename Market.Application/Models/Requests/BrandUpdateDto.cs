namespace Market.Application.Models.Requests;

public class BrandUpdateDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
