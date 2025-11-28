using InventoryService.Application.Interfaces;
using InventoryService.Application.Services;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//var sqlConn = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") ?? "Server=(localdb)\\mssqllocaldb;Database=Inventory;Trusted_Connection=True;TrustServerCertificate=True;";
//var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL") ?? throw new InvalidOperationException("Redis URL required");

var sqlConn = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("Missing DefaultConnection");

var redisUrl = builder.Configuration["Redis:Url"]
               ?? throw new InvalidOperationException("Missing Redis:Url");

builder.Services.AddDbContext<InventoryDbContext>(options => options.UseSqlServer(sqlConn));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisUrl));

builder.Services.AddScoped<IInventoryService, InventoryService.Application.Services.InventoryService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

builder.Services.AddHostedService<ProductEventConsumer>();

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
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();