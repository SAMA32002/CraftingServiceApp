using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Helpers;
using CraftingServiceApp.AdminAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketController> _logger;

    public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ContentContainer<IEnumerable<TicketDto>>>> GetTickets()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(new ContentContainer<IEnumerable<TicketDto>>(tickets, "Tickets retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentContainer<TicketDto>>> GetTicket(int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound(new ContentContainer<TicketDto>(null, "Ticket not found"));
        return Ok(new ContentContainer<TicketDto>(ticket, "Ticket retrieved successfully"));
    }

    [HttpPost]
    public async Task<ActionResult<ContentContainer<TicketDto>>> CreateTicket([FromBody] CreateTicketDto ticketDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ContentContainer<TicketDto>(null, "Invalid ticket data"));
        var createdTicket = await _ticketService.CreateTicketAsync(ticketDto);
        return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, new ContentContainer<TicketDto>(createdTicket, "Ticket created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditTicket(int id, [FromBody] UpdateTicketDto ticketDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ContentContainer<bool>(false, "Invalid ticket data"));
        var result = await _ticketService.UpdateTicketAsync(id, ticketDto);
        if (!result) return NotFound(new ContentContainer<bool>(false, "Ticket not found"));
        return Ok(new ContentContainer<bool>(true, "Ticket updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var result = await _ticketService.DeleteTicketAsync(id);
        if (!result) return NotFound(new ContentContainer<bool>(false, "Ticket not found"));
        return Ok(new ContentContainer<bool>(true, "Ticket deleted successfully"));
    }
}