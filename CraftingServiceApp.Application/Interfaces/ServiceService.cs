using CraftingServiceApp.Domain.Entities;
using System;

namespace CraftingServiceApp.Application.Interfaces
{
    public class ServiceService : IServiceService
    {
        private readonly IRepository<Service> _serviceRepository;

        public ServiceService(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public IEnumerable<Service> GetServicesByCategory(int categoryId)
        {
            return _serviceRepository.Find(s => s.CategoryId == categoryId);
        }

        public IEnumerable<Service> GetServicesByCrafter(string crafterId)
        {
            return _serviceRepository.Find(s => s.CrafterId == crafterId);
        }

        public IEnumerable<Service> GetTopRatedServices(int count)
        {
            return _serviceRepository.GetAll()
                .OrderByDescending(s => s.Reviews.Count != 0 ? s.Reviews.Average(r => r.Rating) : 0)
                .Take(count);
        }

    }

}
