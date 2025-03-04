using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IServiceService
    {
        IEnumerable<Service> GetServicesByCategory(int categoryId);
        IEnumerable<Service> GetServicesByCrafter(string crafterId);
        IEnumerable<Service> GetTopRatedServices(int count);
    }
}
