using EmployeeAnalytics.Application.Analytics;
using Microsoft.AspNetCore.Http;

namespace EmployeeAnalytics.Application.Upload;

public interface IUploadService
{
    List<EmployeePerformance> ParseEmployeePerformance(IFormFile file);
    Task<Guid?> UploadFileAsync(IFormFile file, Guid userId, CancellationToken token);
    
    Task<IEnumerable<Upload>> GetUserUploadsAsync(Guid userId, CancellationToken token);
    Task<IEnumerable<Upload>> GetAllUploadsAsync(CancellationToken token);
    
    Task<IEnumerable<RawMetrics>> GetMetricsAsync(Guid requestedUserId, Guid uploadId,bool isAdmin, CancellationToken token = default);
}