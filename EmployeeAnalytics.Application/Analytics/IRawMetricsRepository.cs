namespace EmployeeAnalytics.Application.Analytics;

public interface IRawMetricsRepository
{
    Task<bool> CreateRawMetricsBulkAsync(IEnumerable<RawMetrics> rawMetrics, CancellationToken token = default);
    Task<IEnumerable<RawMetrics>> GetAllRawMetricsAsync(CancellationToken token = default);
    Task<IEnumerable<RawMetrics>> GetRawMetricByUploadIdAsync(Guid uploadId, CancellationToken token = default);
}