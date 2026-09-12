using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using OrdersApi.Services;
using Xunit;

namespace OrdersApi.Tests;

public sealed class OrdersApiIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_existing_order_returns_the_order()
    {
        var response = await _client.GetAsync($"/orders/{OrderStore.SeedOrderId}");

        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.Equal(OrderStore.SeedOrderId, order!.Id);
        Assert.Equal("Confirmed", order.Status);
        Assert.Equal(4998, order.Total.Amount);
    }

    [Fact]
    public async Task Get_unknown_order_returns_not_found_problem()
    {
        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Post_valid_order_creates_a_pending_order()
    {
        var response = await _client.PostAsJsonAsync("/orders", new
        {
            customerId = "cus_newcustomer",
            items = new[]
            {
                new { sku = "notebook-a5", name = "A5 Notebook", quantity = 2, unitPrice = new { amount = 2499, currency = "ZAR" } }
            }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.Equal("Pending", order!.Status);
        Assert.Equal(4998, order.Total.Amount);
    }

    [Fact]
    public async Task Post_invalid_order_returns_validation_problem()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { customerId = "invalid", items = Array.Empty<object>() });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task Swagger_document_is_generated_with_the_order_schema()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        var document = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        Assert.Contains("Order", document);
        Assert.Contains("OrderStatus", document);
        Assert.Contains("pending", document, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class OrderResponse
    {
        public Guid Id { get; init; }
        public required string Status { get; init; }
        public required MoneyResponse Total { get; init; }
    }

    private sealed class MoneyResponse
    {
        public int Amount { get; init; }
    }
}
