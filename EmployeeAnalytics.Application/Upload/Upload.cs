namespace EmployeeAnalytics.Application.Upload;

public class Upload
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime UploadDate { get; set; }
    public int RowsProcessed { get; set; }

}
