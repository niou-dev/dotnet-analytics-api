using EmployeeAnalytics.Contracts.Analytics.Charts;

namespace EmployeeAnalytics.Contracts.Analytics;

public class AnalyticsResponse
{
    public double AvgTasks {get; set;}
    public double AvgHours {get; set;}
    public double AvgEfficiency {get; set;}
    public int TotalEmployees {get; set;}
    
    // charts
    public required LineChart LineChart { get; set; }
    public required BarChart BarChart { get; set; }
    public required PieChart PieChart { get; set; }
    
    // table
    public required List<TopPerformer> TopPerformers { get; set; }
}