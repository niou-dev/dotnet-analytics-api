using System.Text;
using EmployeeAnalytics.Application.Analytics;
using EmployeeAnalytics.Application.Auth;
using EmployeeAnalytics.Application.Database;
using EmployeeAnalytics.Application.Upload;
using EmployeeAnalytics.Application.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var config =  builder.Configuration;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container.

var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
if (string.IsNullOrEmpty(connectionString)) throw new Exception("Please specify a connection string");

builder.Services.AddSingleton<IDbConnectionFactory>(_ => 
    new NpgsqlConnectionFactory(connectionString));

builder.Services.AddSingleton<DbInitializer>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("Admin",
        p => p.RequireClaim("admin", "true"));
});


builder.Services.AddControllers();


// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IUploadRepository, UploadRepository>();
builder.Services.AddScoped<IRawMetricsRepository, RawMetricsRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// Repositories
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();


app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// db init
var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();