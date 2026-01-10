namespace EmployeeAnalytics.Application.Analytics;


public class EmployeePerformance
{
    public int EmployeeId { get; set; }
    public required string Name { get; set; }
    public required string Department { get; set; }
    public int TasksCompleted { get; set; }
    public double HoursWorked { get; set; }
    public DateTime Date { get; set; }
}