using EmployeeAnalytics.Application.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAnalytics.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/upload")]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly string[] _allowedContentTypes = new[]
    {
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
        "application/vnd.ms-excel" // .xls
    };

    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Upload(IFormFile file, CancellationToken token)
    {
        
        var userId = Guid.Parse(User.FindFirst("userid")!.Value);
        
        if (file == null! || file.Length == 0) return BadRequest();
        if (!_allowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest("File type is not supported. Please upload an Excel file.");
        }
        
        try
        {
            var uploadData = _uploadService.ParseEmployeePerformance(file);
            var uploadId = await _uploadService.UploadFileAsync(file, userId, token);
            if (uploadId == null) throw new Exception("Upload failed.");
            return Ok(new
            {
                uploadId = uploadId
            });
        }
        catch  (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("me")]
    public async Task<IActionResult> GetMyUploads(CancellationToken token)
    {
        var userId = Guid.Parse(User.FindFirst("userid")!.Value);
        var userUploads = await _uploadService.GetUserUploadsAsync(userId, token);
        return Ok(userUploads);
    }

    [HttpGet("my-metrics")]
    public async Task<IActionResult> GetMySpecificUploadRawMetric([FromQuery]Guid uploadId, CancellationToken token)
    {
        var userId = Guid.Parse(User.FindFirst("userid")!.Value);
        var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "admin")!.Value == "true";
        
        try
        {
            return Ok(await _uploadService.GetMetricsAsync(userId, uploadId, isAdmin, token));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize("Admin")]
    [HttpGet("all-uploads")]
    public async Task<IActionResult> GetAllUploads(CancellationToken token)
    {
        var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "admin")!.Value == "true";
        var result = await _uploadService.GetAllUploadsAsync(token);
        return Ok(result);
    }
    
}