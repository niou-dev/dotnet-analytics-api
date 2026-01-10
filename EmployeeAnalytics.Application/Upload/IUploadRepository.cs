namespace EmployeeAnalytics.Application.Upload;

public interface IUploadRepository
{
    Task<bool> CreateUploadAsync(Upload upload, CancellationToken token = default);
    Task<Upload?> GetUploadByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Upload>> GetUploadsByUserIdAsync(Guid userId,CancellationToken token = default); // /me/uploads h /uploads/me
    Task<IEnumerable<Upload>> GetAllUploadsAsync(CancellationToken token = default); // admin only
    
}