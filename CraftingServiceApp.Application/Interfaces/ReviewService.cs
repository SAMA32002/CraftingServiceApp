using CraftingServiceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;

        public ReviewService(IRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public IEnumerable<Review> GetReviewsByService(int serviceId)
        {
            return _reviewRepository.Find(r => r.ServiceId == serviceId);
        }

        public IEnumerable<Review> GetReviewsByClient(string clientId)
        {
            return _reviewRepository.Find(r => r.ClientId == clientId);
        }

        public double GetAverageRating(int serviceId)
        {
            var reviews = _reviewRepository.Find(r => r.ServiceId == serviceId);
            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }
    }

}
