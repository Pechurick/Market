namespace Market.Application.Models.Requests;

public class CategoryCreateDto
{
	public Guid? ParentId { get; set; }
	public required string Name { get; set; }
}
