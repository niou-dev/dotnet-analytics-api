using EmployeeAnalytics.Application.Users;
using EmployeeAnalytics.Contracts.Users;

namespace EmployeeAnalytics.Application.Mapping;

public static class ContractMapping
{
    
    public static User MapToUser(this CreateUserRequest createUserRequest)
    {
        User user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserRequest.Username,
            Password = "",
            Email = createUserRequest.Email,
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        return user;
    }

    public static UserResponse MapToUserResponse(this User user)
    {
        return new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}