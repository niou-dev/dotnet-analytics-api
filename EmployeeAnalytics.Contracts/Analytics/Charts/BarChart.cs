namespace EmployeeAnalytics.Contracts.Analytics.Charts;

public class BarChart
{
    public List<string> Labels { get; set; } = new();
    public List<BarChartDataset> Datasets { get; set; } = new();
}