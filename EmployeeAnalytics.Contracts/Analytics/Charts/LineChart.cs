namespace EmployeeAnalytics.Contracts.Analytics.Charts;

public class LineChart
{
    public List<string> Labels { get; set; } = new();
    public List<double> Values { get; set; } = new();
}