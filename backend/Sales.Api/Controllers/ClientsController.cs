using Microsoft.AspNetCore.Mvc;
using Sales.Application.DTOs.Clients;
using Sales.Application.Interfaces;

namespace Sales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClientResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetAllAsync(
            cancellationToken);

        return Ok(clients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var client = await _clientService.GetByIdAsync(
            id,
            cancellationToken);

        if (client is null)
        {
            return NotFound();
        }

        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        var client = await _clientService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = client.Id },
            client);
    }
}