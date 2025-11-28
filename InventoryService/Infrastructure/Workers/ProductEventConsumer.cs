using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Persistence;
using SharedContracts.Events;
using StackExchange.Redis;
using System.Text.Json;

namespace InventoryService.Infrastructure.Workers
{
    public class ProductEventConsumer : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<ProductEventConsumer> _logger;
        private readonly IServiceProvider _sp;
        private const string StreamName = "product-events";
        private const string GroupName = "inventory-service";

        public ProductEventConsumer(IConnectionMultiplexer redis, ILogger<ProductEventConsumer> logger, IServiceProvider sp)
        {
            _redis = redis;
            _logger = logger;
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();

            // Ensure stream and consumer group exist
            if (!await db.KeyExistsAsync(StreamName))
            {
                await db.StreamAddAsync(StreamName, new NameValueEntry[] { new("init", "stream created") });
            }

            var groups = await db.StreamGroupInfoAsync(StreamName);
            if (!groups.Any(g => g.Name == GroupName))
            {
                await db.StreamCreateConsumerGroupAsync(StreamName, GroupName, "0-0");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var entries = await db.StreamReadGroupAsync(StreamName, GroupName, Environment.MachineName, StreamPosition.NewMessages);

                if (entries.Length == 0)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                foreach (var entry in entries)
                {
                    var eventType = entry.Values.FirstOrDefault(v => v.Name == "eventType").Value;
                    var data = entry.Values.FirstOrDefault(v => v.Name == "data").Value;

                    if (data.IsNullOrEmpty || eventType != nameof(ProductCreatedEvent)) continue;

                    try
                    {
                        var evt = JsonSerializer.Deserialize<ProductCreatedEvent>(data!);

                        using var scope = _sp.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                        if (await context.Inventories.FindAsync(evt.ProductId) != null)
                        {
                            _logger.LogWarning("Inventory already exists for Product {ProductId}", evt.ProductId);
                            await db.StreamAcknowledgeAsync(StreamName, GroupName, entry.Id);
                            continue;
                        }

                        context.Inventories.Add(new Inventory
                        {
                            ProductId = evt.ProductId,
                            Quantity = 100,
                            LastUpdated = DateTime.UtcNow
                        });

                        await context.SaveChangesAsync();

                        await db.StreamAcknowledgeAsync(StreamName, GroupName, entry.Id);

                        _logger.LogInformation("Initial stock created for Product {ProductId}", evt.ProductId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process ProductCreatedEvent");
                        // Optionally, add to a dead-letter queue or retry logic
                    }
                }
            }
        }
    }
}