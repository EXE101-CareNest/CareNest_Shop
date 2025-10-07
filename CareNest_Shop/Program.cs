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
dbSettings.Display();
string connectionString = dbSettings?.GetConnectionString()
                        ?? "Host=localhost;Port=5432;Database=shop-dev;Username=exe-carenest-dev;Password=nghi123";

// Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
