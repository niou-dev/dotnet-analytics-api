namespace EmployeeAnalytics.Contracts.Analytics.Charts;

public class TopPerformer
{
    public required string Name {get; set;}
    public required string Department { get; set; }
    public int Tasks { get; set; }
    public double Efficiency { get; set; }
}