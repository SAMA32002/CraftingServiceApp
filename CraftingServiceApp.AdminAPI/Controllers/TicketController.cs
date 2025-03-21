using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Helpers;
using CraftingServiceApp.AdminAPI.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketController> _logger;
    private readonly EmailService _emailService;
    private readonly ApplicationDbContext _context;

    public TicketController(ITicketService ticketService, ILogger<TicketController> logger , EmailService emailService , ApplicationDbContext context)
    {
        _ticketService = ticketService;
        _logger = logger;
        _emailService = emailService;
        _context = context;

    }

    [HttpPost("resolve/{ticketId}")]
    public async Task<IActionResult> ResolveTicket(int ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
        {
            return NotFound(new { error = "Ticket not found" });
        }

        ticket.Status = TicketStatus.Resolved;

        await _context.SaveChangesAsync();

        var success = await _emailService.SendEmailAsync(
            ticket.Email,
            "Your Ticket Has Been Resolved!",
            "<h2>Hello,</h2><p>We want to inform you that your ticket has been successfully resolved ✅.</p>"
        );

        if (success)
        {
            return Ok(new { message = "Ticket resolved and email sent." });
        }

        return BadRequest(new { error = "Ticket resolved but email failed to send." });
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

    //[HttpPost]
    //public async Task<ActionResult<ContentContainer<TicketDto>>> CreateTicket([FromBody] CreateTicketDto ticketDto)
    //{
    //    if (!ModelState.IsValid) return BadRequest(new ContentContainer<TicketDto>(null, "Invalid ticket data"));
    //    var createdTicket = await _ticketService.CreateTicketAsync(ticketDto);
    //    return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, new ContentContainer<TicketDto>(createdTicket, "Ticket created successfully"));
    //}

    //[HttpPut("{id}")]
    //public async Task<IActionResult> EditTicket(int id, [FromBody] UpdateTicketDto ticketDto)
    //{
    //    if (!ModelState.IsValid) return BadRequest(new ContentContainer<bool>(false, "Invalid ticket data"));
    //    var result = await _ticketService.UpdateTicketAsync(id, ticketDto);
    //    if (!result) return NotFound(new ContentContainer<bool>(false, "Ticket not found"));
    //    return Ok(new ContentContainer<bool>(true, "Ticket updated successfully"));
    //}

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var result = await _ticketService.DeleteTicketAsync(id);
        if (!result) return NotFound(new ContentContainer<bool>(false, "Ticket not found"));
        return Ok(new ContentContainer<bool>(true, "Ticket deleted successfully"));
    }
}