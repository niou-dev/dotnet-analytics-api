using EmployeeAnalytics.Application.Upload;
using EmployeeAnalytics.Contracts.Analytics;
using EmployeeAnalytics.Contracts.Analytics.Charts;

namespace EmployeeAnalytics.Application.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUploadService _uploadService;
    public AnalyticsService(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }


    public async Task<AnalyticsResponse> GenerateUploadAnalyticsAsync(Guid userId, Guid uploadId, bool isAdmin, CancellationToken token = default)
    {
        var data = await _uploadService.GetMetricsAsync(userId,  uploadId, isAdmin, token);
        
        // upologismos kai analush

        // line chart
        var lineGroups = data
            .GroupBy(x => x.MetricDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key,
                AvgTasks = g.Average(x => x.TasksCompleted)
            }).ToList();

        var lineChart = new LineChart
        {
            Labels = lineGroups
                .Select(x => x.Date.ToString("yyyy-MM-dd"))
                .ToList(),

            Values = lineGroups
                .Select(x => Math.Round(x.AvgTasks, 2))
                .ToList()
        };
        
        
        // bar chart
        var barGroups = data
            .GroupBy(x => new
            {
                x.MetricDate.Year,
                x.MetricDate.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var perEmployee = g
                    .GroupBy(x => x.EmployeeId)
                    .Select(e => new
                    {
                        TotalTasks = e.Sum(x => x.TasksCompleted),
                        TotalHours = e.Sum(x => x.HoursWorked)
                    })
                    .ToList();

                return new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    AvgTasksPerEmployee = perEmployee.Any()
                        ? perEmployee.Average(e => e.TotalTasks)
                        : 0,

                    AvgHoursPerEmployee = perEmployee.Any()
                        ? perEmployee.Average(e => e.TotalHours)
                        : 0
                };
            })
            .ToList();

        
        var labelsBar = barGroups
            .Select(x => x.Month.ToString("MM/yyyy"))
            .ToList();


        var avgTasksDataset = new BarChartDataset
        {
            Label = "Avg Tasks / Employee",
            Values = barGroups
                .Select(x => Math.Round(x.AvgTasksPerEmployee, 2))
                .ToList()
        };

        var avgHoursDataset = new BarChartDataset
        {
            Label = "Avg Hours / Employee",
            Values = barGroups
                .Select(x => Math.Round(x.AvgHoursPerEmployee, 2))
                .ToList()
        };


        var barChart = new BarChart
        {
            Labels = labelsBar,
            Datasets = new List<BarChartDataset>()
            {
                avgTasksDataset,
                avgHoursDataset
            }
        };

        
        // pie chart
        var pieData = data
            .GroupBy(x => x.Department)
            .Select(g => new
            {
                Department = g.Key,
                TotalTasks = g.Sum(x => x.TasksCompleted),
            })
            .OrderByDescending(x => x.TotalTasks)
            .ToList();

        var pieChart = new PieChart
        {
            Labels = pieData.Select(x => x.Department).ToList(),
            Values = pieData.Select(x => x.TotalTasks).ToList()
        };
        
        // top3 performers
        
        var top3Performers = data
            .GroupBy(r => new {r.EmployeeName, r.Department})
            .Select(g => new
            {
                Name = g.Key.EmployeeName,
                Department = g.Key.Department,
                Tasks = g.Sum(x => x.TasksCompleted),
                Efficiency = g.Sum( x=> x.HoursWorked) == 0
                ? 0
                : g.Sum(x => x.TasksCompleted) / g.Sum(x => x.HoursWorked)
            })
            .OrderByDescending(x => x.Efficiency)
            .ThenByDescending(x => x.Tasks)
            .Take(3)
            .ToList();
        
        var topPerformers = top3Performers.Select(x => new TopPerformer
        {
            Name = x.Name,
            Department = x.Department,
            Tasks = x.Tasks,
            Efficiency = Math.Round(x.Efficiency * 100, 2)
        }).ToList();
        
        double avgTasks = Math.Round(data.Average(x => x.TasksCompleted), 2);
        double avgHours = Math.Round(data.Average(x => x.HoursWorked), 2);
        
        var totalTasks = data.Sum(x => x.TasksCompleted);
        var totalHours = data.Sum(x => x.HoursWorked);
        double avgEfficiency = totalHours == 0
            ? 0
            : totalTasks / totalHours;
        
        avgEfficiency = Math.Round(avgEfficiency * 100, 2);

        int totalEmployees = data
            .Select(x => x.EmployeeId)
            .Distinct()
            .Count();
        
        return new AnalyticsResponse
        {
            AvgTasks = avgTasks,
            AvgHours = avgHours,
            AvgEfficiency = avgEfficiency,
            TotalEmployees =  totalEmployees,
            LineChart = lineChart,
            BarChart = barChart,
            PieChart = pieChart,
            TopPerformers = topPerformers
        };
        
        
        // telos upologismou kai analyshs
        
        /*List<string> labels = new List<string>() {"date1", "date2", "date3"};
        List<double> values = new List<double>() { 12.3, 45.2, 89.3 };
        LineChart lineChart = new LineChart() {Labels = labels, Values = values};
        
        List<string> labelsBar = new List<string>() {"IT", "Sales", "HR"};
        BarChartDataset dataset1 = new BarChartDataset()
        {
            Label = "Total tasks Completed",
            Values = new List<double>() { 27, 97, 20 }
        };
        BarChartDataset dataset2 = new BarChartDataset()
        {
            Label = "Average tasks per employee",
            Values = new List<double>() { 13.2, 12.7, 22.1 }
        };
        
        
        BarChart barChart = new BarChart()
        {
            Labels = labelsBar,
            Datasets = new List<BarChartDataset>(){dataset1, dataset2}    
        };
        
        AnalyticsResponse analytics =  new AnalyticsResponse()
        {
            LineChart = lineChart,
            BarChart = barChart,
        };*/
        
    }
}