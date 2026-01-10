using EmployeeAnalytics.Contracts.Users;

namespace EmployeeAnalytics.Contracts.Auth;

public class SignupResponse
{
    public required UserResponse UserResponse { get; set; }
    public required string Jwt { get; set; }
}