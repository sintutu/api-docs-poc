using System.Collections.Concurrent;
using OrdersApi.Models;

namespace OrdersApi.Services;

/// <summary>Small in-memory order store for this local learning project.</summary>
public sealed class OrderStore
{
    /// <summary>Identifier of the order seeded solely for local experimentation.</summary>
    public static readonly Guid SeedOrderId = Guid.Parse("84f5e6b0-b2bd-4dc4-8ab1-5b105c0986c8");

    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    /// <summary>Initializes the in-memory store with one retrievable example order.</summary>
    public OrderStore()
    {
        var seed = new Order
        {
            Id = SeedOrderId,
            CustomerId = "cus_9f73b84d",
            Status = OrderStatus.Confirmed,
            Items =
            [
                new OrderItem
                {
                    Sku = "notebook-a5",
                    Name = "A5 Notebook",
                    Quantity = 2,
                    UnitPrice = new Money { Amount = 2499, Currency = "ZAR" }
                }
            ],
            Total = new Money { Amount = 4998, Currency = "ZAR" },
            CreatedAt = new DateTimeOffset(2026, 9, 12, 9, 30, 0, TimeSpan.Zero)
        };
        _orders[seed.Id] = seed;
    }

    /// <summary>Returns an order when it exists in the in-memory store.</summary>
    public Order? Find(Guid id) => _orders.GetValueOrDefault(id);

    /// <summary>Creates and stores an order with a calculated total.</summary>
    public Order Create(CreateOrderRequest request)
    {
        var currency = request.Items[0].UnitPrice.Currency;
        var total = request.Items.Sum(item => item.UnitPrice.Amount * item.Quantity);
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Status = OrderStatus.Pending,
            Items = request.Items,
            Total = new Money { Amount = total, Currency = currency },
            CreatedAt = DateTimeOffset.UtcNow
        };
        _orders[order.Id] = order;
        return order;
    }
}
