using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.Orders;
using Sales.Application.Interfaces;

namespace Sales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetAll(
        [FromQuery] int? clientId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(
            clientId,
            dateFrom,
            dateTo,
            cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(
            id,
            cancellationToken);

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpPost("{id:int}/items")]
    public async Task<ActionResult<OrderResponse>> AddItem(
        int id,
        [FromBody] AddOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.AddItemAsync(
            id,
            request,
            cancellationToken);

        return Ok(order);
    }

    [HttpPut("{id:int}/items/{productId:int}")]
    public async Task<ActionResult<OrderResponse>> UpdateItem(
        int id,
        int productId,
        [FromBody] UpdateOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateItemAsync(
            id,
            productId,
            request,
            cancellationToken);

        return Ok(order);
    }

    [HttpDelete("{id:int}/items/{productId:int}")]
    public async Task<ActionResult<OrderResponse>> RemoveItem(
        int id,
        int productId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.RemoveItemAsync(
            id,
            productId,
            cancellationToken);

        return Ok(order);
    }
}
