using EmployeeAnalytics.Contracts.Analytics;

namespace EmployeeAnalytics.Application.Analytics;

public interface IAnalyticsService
{
    Task<AnalyticsResponse> GenerateUploadAnalyticsAsync(Guid userId, Guid uploadId, bool isAdmin, CancellationToken token = default);
}