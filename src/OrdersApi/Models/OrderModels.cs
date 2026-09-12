using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Models;

/// <summary>A customer order with its current lifecycle state and calculated total.</summary>
public sealed class Order
{
    /// <summary>Immutable identifier assigned when the order is created.</summary>
    public required Guid Id { get; init; }

    /// <summary>Identifier of the customer who placed the order.</summary>
    public required string CustomerId { get; init; }

    /// <summary>Current lifecycle state. New orders begin as pending.</summary>
    public required OrderStatus Status { get; init; }

    /// <summary>Items captured when the order was created.</summary>
    public required IReadOnlyList<OrderItem> Items { get; init; }

    /// <summary>Total of all item prices, expressed as structured money.</summary>
    public required Money Total { get; init; }

    /// <summary>UTC timestamp at which the API accepted the order.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
}

/// <summary>Input required to create an order.</summary>
public sealed class CreateOrderRequest
{
    /// <summary>Customer identifier in the synthetic customer namespace.</summary>
    [Required]
    [RegularExpression("^cus_[A-Za-z0-9]+$", ErrorMessage = "CustomerId must start with 'cus_'.")]
    public required string CustomerId { get; init; }

    /// <summary>One or more priced items to purchase.</summary>
    [Required]
    [MinLength(1)]
    public required List<OrderItem> Items { get; init; }
}

/// <summary>A purchasable item and the agreed price for a single unit.</summary>
public sealed class OrderItem
{
    /// <summary>Merchant stock-keeping unit.</summary>
    [Required]
    [RegularExpression("^[a-z0-9-]+$")]
    public required string Sku { get; init; }

    /// <summary>Customer-facing item name.</summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; init; }

    /// <summary>Number of units ordered.</summary>
    [Range(1, 100)]
    public required int Quantity { get; init; }

    /// <summary>Price for one unit, represented as structured money.</summary>
    [Required]
    public required Money UnitPrice { get; init; }
}

/// <summary>A monetary value in minor currency units, avoiding floating-point ambiguity.</summary>
public sealed class Money
{
    /// <summary>Amount in the minor unit of the currency; 2499 ZAR represents R24.99.</summary>
    [Range(0, int.MaxValue)]
    public required int Amount { get; init; }

    /// <summary>ISO 4217 three-letter currency code.</summary>
    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public required string Currency { get; init; }
}

/// <summary>Lifecycle state of an order.</summary>
public enum OrderStatus
{
    /// <summary>Order was accepted and awaits confirmation.</summary>
    Pending,
    /// <summary>Order is confirmed and awaits fulfilment.</summary>
    Confirmed,
    /// <summary>Order has been fulfilled.</summary>
    Fulfilled,
    /// <summary>Order was cancelled and will not be fulfilled.</summary>
    Cancelled
}
