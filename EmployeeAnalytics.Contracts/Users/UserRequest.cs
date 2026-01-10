namespace EmployeeAnalytics.Contracts.Users;

public class UserRequest
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}