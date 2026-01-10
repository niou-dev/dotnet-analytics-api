namespace EmployeeAnalytics.Contracts.Users;

public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}