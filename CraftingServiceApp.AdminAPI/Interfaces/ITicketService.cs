using CraftingServiceApp.AdminAPI.Dtos;

namespace CraftingServiceApp.AdminAPI.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<TicketDto> GetTicketByIdAsync(int id);
        Task<TicketDto> CreateTicketAsync(CreateTicketDto ticketDto);
        Task<bool> UpdateTicketAsync(int id, UpdateTicketDto ticketDto);
        Task<bool> DeleteTicketAsync(int id);
    }
}
