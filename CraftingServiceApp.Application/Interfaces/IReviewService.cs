using CraftingServiceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IReviewService
    {
        IEnumerable<Review> GetReviewsByService(int serviceId);
        IEnumerable<Review> GetReviewsByClient(string clientId);
        double GetAverageRating(int serviceId);
    }
}
