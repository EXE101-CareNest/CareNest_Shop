using CareNest_Shop.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shop.Application.Common;
using Shop.Application.Features.Commands.Create;
using Shop.Application.Features.Commands.Delete;
using Shop.Application.Features.Commands.Update;
using Shop.Application.Features.Queries.GetAllPaging;
using Shop.Application.Features.Queries.GetById;
using Shop.Application.Interfaces.CQRS;
using Shop.Application.Interfaces.CQRS.Commands;
using Shop.Application.Interfaces.CQRS.Queries;
using Shop.Application.Interfaces.Services;
using Shop.Application.Interfaces.UOW;
using Shop.Application.UseCases;
using Shop.Domain.Repositories;
using Shop.Infrastructure.Persistences.Configuration;
using Shop.Infrastructure.Persistences.Database;
using Shop.Infrastructure.Persistences.Repository;
using Shop.Infrastructure.Services;
using Shop.Infrastructure.UOW;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Lấy DatabaseSettings từ configuration
DatabaseSettings dbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>()!;

// Ở môi trường Production, các giá trị trong appsettings có dạng ${DB_*} sẽ không tự mở rộng.
// Chủ động đọc biến môi trường và ghi đè nếu thấy placeholder hoặc giá trị trống.
string? envHost = Environment.GetEnvironmentVariable("DB_HOST");
string? envPort = Environment.GetEnvironmentVariable("DB_PORT");
string? envUser = Environment.GetEnvironmentVariable("DB_USER");
string? envPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
string? envDatabase = Environment.GetEnvironmentVariable("DB_NAME");

bool IsPlaceholder(string? value) => !string.IsNullOrWhiteSpace(value) && value!.TrimStart().StartsWith("${");

if (IsPlaceholder(dbSettings.Ip) || string.IsNullOrWhiteSpace(dbSettings.Ip))
{
    dbSettings.Ip = string.IsNullOrWhiteSpace(envHost) ? dbSettings.Ip : envHost;
}

if (dbSettings.Port == 0 || IsPlaceholder(dbSettings.Port.ToString()))
{
    if (int.TryParse(envPort, out var parsedPort))
    {
        dbSettings.Port = parsedPort;
    }
    else
    {
        dbSettings.Port = 5432;
    }
}

if (IsPlaceholder(dbSettings.User) || string.IsNullOrWhiteSpace(dbSettings.User))
{
    dbSettings.User = string.IsNullOrWhiteSpace(envUser) ? dbSettings.User : envUser;
}

if (IsPlaceholder(dbSettings.Password) || string.IsNullOrWhiteSpace(dbSettings.Password))
{
    dbSettings.Password = string.IsNullOrWhiteSpace(envPassword) ? dbSettings.Password : envPassword;
}

if (IsPlaceholder(dbSettings.Database) || string.IsNullOrWhiteSpace(dbSettings.Database))
{
    dbSettings.Database = string.IsNullOrWhiteSpace(envDatabase) ? dbSettings.Database : envDatabase;
}

dbSettings.Display();

string connectionString = (dbSettings?.GetConnectionString()
                        ?? "Host=localhost;Port=5432;Database=shop-dev;Username=exe-carenest-dev;Password=nghi123")
                        + ";Pooling=true;Maximum Pool Size=5;Minimum Pool Size=0;Timeout=15;";

// Xử lý biến môi trường cho APIService
var apiServiceSection = builder.Configuration.GetSection("APIService");
if (apiServiceSection.Exists())
{
    string? apiBaseUrl = apiServiceSection["BaseUrlAccount"];
    if (IsPlaceholder(apiBaseUrl) || string.IsNullOrWhiteSpace(apiBaseUrl))
    {
        string? envApiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
        if (!string.IsNullOrWhiteSpace(envApiBaseUrl))
        {
            builder.Configuration["APIService:BaseUrlAccount"] = envApiBaseUrl;
        }
    }
}

// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
        // npgsqlOptions.CommandTimeout(60); // tuỳ chọn khi cần
    }));

builder.Services.AddTransient<DatabaseSeeder>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Đăng ký service thêm chú thích cho api
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    //ADD JWT BEARER SECURITY DEFINITION
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        //Type = SecuritySchemeType.ApiKey,
        Type = SecuritySchemeType.Http,//ko cần thêm token phía trước
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer"
            },
            new List<string>()
        }
    });
});

// Đăng ký các repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//command
builder.Services.AddScoped<ICommandHandler<CreateCommand, Shop.Domain.Entitites.Shop>, CreateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCommand, Shop.Domain.Entitites.Shop>, UpdateCommandHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCommand>, DeleteCommandHandler>();
//query
builder.Services.AddScoped<IQueryHandler<GetAllPagingQuery, PageResult<ShopResponse>>, GetAllPagingQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetByIdQuery, Shop.Domain.Entitites.Shop>, GetByIdQueryHandler>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);


//Đăng ký cho FE

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});



//Đăng ký lấy thông tin từ token
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
var swaggerEnabled = app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled");
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS");
    if (!string.IsNullOrWhiteSpace(runMigrations) && runMigrations.Equals("true", StringComparison.OrdinalIgnoreCase))
    {
        context.Database.Migrate();
    }
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
