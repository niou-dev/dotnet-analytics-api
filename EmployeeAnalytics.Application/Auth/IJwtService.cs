using EmployeeAnalytics.Application.Users;

namespace EmployeeAnalytics.Application.Auth;

public interface IJwtService
{
    string GenerateToken(User user, Dictionary<string, object>? customClaims = null);
}