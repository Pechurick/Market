namespace Market.Application.Models.Responses;

public class CategoryListDto
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public List<CategoryListDto> Categories { get; set; } = [];
}
