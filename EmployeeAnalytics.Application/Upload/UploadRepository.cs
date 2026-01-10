using Dapper;
using EmployeeAnalytics.Application.Database;

namespace EmployeeAnalytics.Application.Upload;

public class UploadRepository : IUploadRepository
{
    
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UploadRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<bool> CreateUploadAsync(Upload upload, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  insert into uploads (id, user_id, upload_date, rows_processed)
                                  values (@Id, @UserId, @UploadDate, @RowsProcessed)
                                  """, upload, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }
    
    public async Task<Upload?> GetUploadByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var upload = await connection.QuerySingleOrDefaultAsync<Upload>(
            new CommandDefinition("""
                                  select
                                      id,
                                  user_id as "UserId",
                                  upload_date as "UploadDate",
                                  rows_processed as "RowsProcessed"
                                      from uploads where id = @id
                                  """, new { id }, cancellationToken: token));
        return upload;
    }

    public async Task<IEnumerable<Upload>> GetUploadsByUserIdAsync(Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var uploads = await connection.QueryAsync<Upload>(
            new CommandDefinition("""
                                  select
                                      id,
                                  user_id as "UserId",
                                  upload_date as "UploadDate",
                                  rows_processed as "RowsProcessed"
                                      from uploads where user_id = @userId
                                  """, new { userId }, cancellationToken: token));
        return uploads;
    }

    public async Task<IEnumerable<Upload>> GetAllUploadsAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var uploads = await connection.QueryAsync<Upload>(
            new CommandDefinition("""
                                  select * from uploads
                                  """, cancellationToken: token));
        return uploads;
    }
    
}