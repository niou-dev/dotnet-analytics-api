using ClosedXML.Excel;
using EmployeeAnalytics.Application.Analytics;
using Microsoft.AspNetCore.Http;

namespace EmployeeAnalytics.Application.Upload;

public class UploadService : IUploadService
{
    
    private readonly IUploadRepository _uploadRepository;
    private readonly IRawMetricsRepository _rawMetricsRepository;

    public UploadService(IUploadRepository uploadRepository, IRawMetricsRepository rawMetricsRepository)
    {
        _uploadRepository = uploadRepository;
        _rawMetricsRepository = rawMetricsRepository;
    }
    
    public List<EmployeePerformance> ParseEmployeePerformance(IFormFile file)
    {
        var result = new List<EmployeePerformance>();

        using var workbook = new XLWorkbook(file.OpenReadStream());
        var worksheet = workbook.Worksheet(1);

        // skip header row
        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var item = new EmployeePerformance
            {
                EmployeeId = row.Cell(1).GetValue<int>(),
                Name = row.Cell(2).GetValue<string>(),
                Department = row.Cell(3).GetValue<string>(),
                TasksCompleted = row.Cell(4).GetValue<int>(),
                HoursWorked = row.Cell(5).GetValue<double>(),
                Date = row.Cell(6).GetValue<DateTime>()
            };

            result.Add(item);
        }
        
        return result;
    }

    public async Task<Guid?> UploadFileAsync(IFormFile file, Guid userId, CancellationToken token)
    {
        var fileData = ParseEmployeePerformance(file);

        var uploadId = Guid.NewGuid();
        Upload uploadInfo = new Upload()
        {
            Id = uploadId,
            UserId = userId,
            UploadDate = DateTime.Now,
            RowsProcessed = fileData.Count,
        };
        
        var result1 = await _uploadRepository.CreateUploadAsync(uploadInfo, token);

        var metrics = fileData.Select(x => new RawMetrics()
        {
            Id = Guid.NewGuid(),
            UploadId = uploadId,
            EmployeeId = x.EmployeeId,
            EmployeeName = x.Name,
            Department = x.Department,
            TasksCompleted = x.TasksCompleted,
            HoursWorked = x.HoursWorked,
            MetricDate = x.Date,
        });
        
        var result2 = await _rawMetricsRepository.CreateRawMetricsBulkAsync(metrics, token);

        if (result1 && result2)
        {
            return uploadId;
        }

        return null;
    }

    public async Task<IEnumerable<Upload>> GetUserUploadsAsync(Guid userId, CancellationToken token)
    {
        return await _uploadRepository.GetUploadsByUserIdAsync(userId, token);
    }

    public async Task<IEnumerable<Upload>> GetAllUploadsAsync(CancellationToken token)
    {
        return await _uploadRepository.GetAllUploadsAsync(token);
    }

    public async Task<IEnumerable<RawMetrics>> GetMetricsAsync(Guid requestedUserId, Guid uploadId,bool isAdmin, CancellationToken token)
    {
        Upload? upload = await _uploadRepository.GetUploadByIdAsync(uploadId, token);
        
        if (upload is null) throw new KeyNotFoundException("Upload not found");
        
        if (!isAdmin && upload.UserId != requestedUserId)
        {
            throw new KeyNotFoundException("upload not found"); // resource hiding
        }
        
        var result = await _rawMetricsRepository.GetRawMetricByUploadIdAsync(uploadId, token);
        
        return result;
    }
}