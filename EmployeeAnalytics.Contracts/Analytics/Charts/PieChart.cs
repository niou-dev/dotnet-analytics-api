namespace EmployeeAnalytics.Contracts.Analytics.Charts;

public class PieChart
{
    public required List<string> Labels { get; set; }
    public required List<int> Values { get; set; }
}