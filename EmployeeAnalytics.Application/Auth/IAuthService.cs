using EmployeeAnalytics.Contracts.Auth;

namespace EmployeeAnalytics.Application.Auth;

public interface IAuthService
{
    Task<SignupResponse> SignUp(SignUpRequest signUpRequest);
    Task<LoginResponse> Login(LoginRequest loginRequest);
}