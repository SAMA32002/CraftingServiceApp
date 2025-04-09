using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TicketService> _logger;

    public TicketService(ApplicationDbContext context, ILogger<TicketService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Select(t => new TicketDto { Id = t.Id, Email = t.Email, Subject = t.Subject, Message = t.Message, Status = t.Status, CreatedAt = t.CreatedAt })
            .ToListAsync();
    }

    public async Task<TicketDto> GetTicketByIdAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return null;
        return new TicketDto { Id = ticket.Id, Email = ticket.Email, Subject = ticket.Subject, Message = ticket.Message, Status = ticket.Status, CreatedAt = ticket.CreatedAt };
    }

    public async Task<TicketDto> CreateTicketAsync(CreateTicketDto ticketDto)
    {
        var ticket = new Ticket
        {
            Email = ticketDto.Email,
            Subject = ticketDto.Subject,
            Message = ticketDto.Message,
            CreatedAt = DateTime.UtcNow,
            Status = TicketStatus.Open
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return new TicketDto { Id = ticket.Id, Email = ticket.Email, Subject = ticket.Subject, Message = ticket.Message, Status = ticket.Status };
    }

    public async Task<bool> UpdateTicketAsync(int id, UpdateTicketDto ticketDto)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return false;

        ticket.Status = ticketDto.Status;
        _context.Entry(ticket).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTicketAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return false;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }
}