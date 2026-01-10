using EmployeeAnalytics.Application.Analytics;
using EmployeeAnalytics.Application.Upload;
using EmployeeAnalytics.Application.Users;
using EmployeeAnalytics.Contracts.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAnalytics.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IUsersService _usersService;
    private readonly IUploadService _uploadService;

    public AnalyticsController(IAnalyticsService analyticsService, IUsersService usersService, IUploadService uploadService)
    {
        _analyticsService = analyticsService;
        _usersService = usersService;
        _uploadService = uploadService;
    }

    [HttpGet]
    public async Task<ActionResult<AnalyticsResponse>> GetUploadAnalytics([FromQuery] Guid uploadId, CancellationToken token)
    {
        var userId = Guid.Parse(User.FindFirst("userid")!.Value);
        var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "admin")!.Value == "true";
        
        AnalyticsResponse response = await _analyticsService.GenerateUploadAnalyticsAsync(userId, uploadId, isAdmin, token);
        return Ok(response);
    }

    [Authorize("Admin")]
    [HttpGet("admin-analytics")]
    public async Task<IActionResult> GetAdminAnalytics(CancellationToken token)
    {
        var users = await _usersService.GetUsers();
        int usersCount = users.Count();

        var uploads = await _uploadService.GetAllUploadsAsync(token);
        int uploadsCount = uploads.Count();

        return Ok(new
        {
            UsersCount = usersCount,
            Users = users,
            UploadsCount = uploadsCount,
            Uploads = uploads
        });
    }
}