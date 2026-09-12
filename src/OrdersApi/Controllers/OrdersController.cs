using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;
using OrdersApi.Services;

namespace OrdersApi.Controllers;

/// <summary>Creates and retrieves synthetic customer orders.</summary>
[ApiController]
[Route("orders")]
public sealed class OrdersController(OrderStore store) : ControllerBase
{
    /// <summary>Retrieves an order by its identifier.</summary>
    /// <param name="orderId">UUID assigned to the order when it was created.</param>
    /// <response code="200">The requested order.</response>
    /// <response code="400">The identifier is not a valid UUID.</response>
    /// <response code="404">No order exists for the identifier.</response>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Order> GetById(Guid orderId)
    {
        var order = store.Find(orderId);
        return order is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Order not found", detail: $"No order exists with ID {orderId}.")
            : Ok(order);
    }

    /// <summary>Creates an order and starts it in the pending lifecycle state.</summary>
    /// <param name="request">Customer and one or more priced items.</param>
    /// <response code="201">The newly created order.</response>
    /// <response code="400">The request fails model validation.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<Order> Create(CreateOrderRequest request)
    {
        var order = store.Create(request);
        return CreatedAtAction(nameof(GetById), new { orderId = order.Id }, order);
    }
}
