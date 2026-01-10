namespace EmployeeAnalytics.Application.Analytics;

public class RawMetrics
{
    public Guid Id { get; set; }
    public Guid UploadId { get; set; }
    public int EmployeeId { get; set; }
    public required string EmployeeName { get; set; }
    public required string Department { get; set; }
    public int TasksCompleted { get; set; }
    public double HoursWorked { get; set; }
    public DateTime MetricDate { get; set; }
    
}