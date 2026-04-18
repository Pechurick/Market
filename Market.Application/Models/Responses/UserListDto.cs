namespace Market.Application.Models.Responses;

public class UserListDto
{
	public long Id { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public string? MiddleName { get; set; }
	public DateOnly? BirthDate { get; set; }
}
