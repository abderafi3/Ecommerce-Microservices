using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//var sqlConn = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") ?? throw new InvalidOperationException("SQL connection string required");
//var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL") ?? throw new InvalidOperationException("Redis URL required");

var sqlConn = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("Missing DefaultConnection");

var redisUrl = builder.Configuration["Redis:Url"]
               ?? throw new InvalidOperationException("Missing Redis:Url");

builder.Services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(sqlConn));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisUrl));

builder.Services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();