using Dapper;

namespace EmployeeAnalytics.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync(
            """
            create table if not exists users(
                id UUID primary key,
                username TEXT not null unique,
                email TEXT not null unique,
                password text not null,
                is_admin boolean not null default false,
                created_at timestamptz not null,
                updated_at timestamptz not null
                );
            create table if not exists uploads(
                id UUID primary key,
                user_id UUID not null,
                upload_date timestamptz not null,
                rows_processed INTEGER not null,
                constraint fk_uploads_user
                    foreign key (user_id)
                    references users(id)
                    on delete cascade
                );

            create table if not exists raw_metrics(
                id UUID primary key,
                upload_id UUID not null,
                external_employee_id TEXT,
                employee_name TEXT,
                department TEXT not null,
                tasks_completed INTEGER not null,
                hours_worked NUMERIC not null,
                metric_date TIMESTAMPTZ not null,
                constraint fk_rawmetrics_upload
                    foreign key (upload_id)
                    references uploads(id)
                    on delete cascade
            );

            create index if not exists idx_rawmetrics_upload
                on raw_metrics(upload_id);
            
            create index if not exists idx_rawmetrics_department
                on raw_metrics(department);
            
            create index if not exists idx_rawmetrics_date
                on raw_metrics(metric_date);
            """
            );
    }
}