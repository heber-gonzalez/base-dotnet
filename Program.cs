global using Core.Models.Auth;
global using API.DTOs.Auth;
global using Core.Interfaces.Repositories;
global using Core.Interfaces.Auth;
global using Core.Interfaces.Logs;
global using Infrastructure.Repositories;
global using Infrastructure.Data.Context;
global using API.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Middlewares;
using System.Reflection;
using Infrastructure.Auth;
using Core.Interfaces.Users;
DotNetEnv.Env.Load();


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

builder.Services.AddScoped<ILogService, LogService>();


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*");
        });
});

builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5224); });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration.GetSection("JWT_SECRET").Value;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret ?? throw new Exception("Secret not found"))),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
    };
})
.AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(
    ApiKeySchemeOptions.Scheme, options =>
    {
        options.HeaderName = "X-API-KEY";
    });

builder.Services.AddDbContext<BaseDbContext>(options =>
{
    var host = Environment.GetEnvironmentVariable("DB_HOST") ?? builder.Configuration.GetSection("DB_HOST").Value;
    var name = Environment.GetEnvironmentVariable("DB_NAME") ?? builder.Configuration.GetSection("DB_NAME").Value;
    var user = Environment.GetEnvironmentVariable("DB_USER") ?? builder.Configuration.GetSection("DB_USER").Value;
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? builder.Configuration.GetSection("DB_PASSWORD").Value;
        options.UseMySql($"Server={host};Database={name};Uid={user};Pwd={password};SslMode=None;ConnectionTimeout=0", ServerVersion.AutoDetect($"Server={host};Database={name};Uid={user};Pwd={password};SslMode=None;ConnectionTimeout=0"));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePathBase("/base");
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<LogsMiddleware>();

//app.UseHttpsRedirection();

app.UseCors(builder => builder
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowAnyOrigin()
    );

app.MapControllers();

app.Run();
