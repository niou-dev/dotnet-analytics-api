namespace EmployeeAnalytics.Contracts.Analytics.Charts;

public class BarChartDataset
{
    public string Label { get; set; } = string.Empty;
    public List<double> Values { get; set; } = new ();
}