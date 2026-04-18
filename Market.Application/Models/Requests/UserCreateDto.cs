namespace Market.Application.Models.Requests;

public class UserCreateDto
{
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public string? MiddleName { get; set; }
	public DateOnly? BirthDate { get; set; }
}
