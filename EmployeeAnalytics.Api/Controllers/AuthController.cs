using EmployeeAnalytics.Application.Auth;
using EmployeeAnalytics.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAnalytics.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/api/v1/auth/login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var result = await _authService.Login(loginRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("/api/v1/auth/signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
    {
        try
        {
            var result = await _authService.SignUp(signUpRequest);
            return Ok(result);
        }
        catch(Exception ex)
        {
            return BadRequest(ex);
        }
        
    }
    
}