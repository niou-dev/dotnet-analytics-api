using Dapper;
using EmployeeAnalytics.Application.Database;

namespace EmployeeAnalytics.Application.Analytics;

public class RawMetricsRepository : IRawMetricsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RawMetricsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateRawMetricsBulkAsync(IEnumerable<RawMetrics> rawMetrics, CancellationToken token = default)
    {
        int metricsCount = rawMetrics.Count();
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        int counter = 0;
        foreach (var line in rawMetrics)
        {
            var result = await connection.ExecuteAsync(
                new CommandDefinition(
                    """
                    insert into raw_metrics (id, upload_id, employee_id, employee_name, department, tasks_completed, hours_worked, metric_date)
                    values (@Id, @UploadId, @EmployeeId, @EmployeeName, @Department, @TasksCompleted, @HoursWorked, @MetricDate)
                    """, line, transaction, cancellationToken: token));
            counter++;

        }
        transaction.Commit();
        
        return counter == metricsCount;
    }

    public async Task<IEnumerable<RawMetrics>> GetAllRawMetricsAsync(CancellationToken token = default)
    {
        using var connection =  await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.QueryAsync<RawMetrics>(
            new CommandDefinition("""
                                  select * from raw_metrics
                                  """));
        return result;
    }

    public async Task<IEnumerable<RawMetrics>> GetRawMetricByUploadIdAsync(Guid uploadId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.QueryAsync<RawMetrics>(
            new CommandDefinition("""
                                  select * from raw_metrics
                                  where  upload_id = @uploadId
                                  """, new {uploadId}, cancellationToken: token));
        return result;
    }
}