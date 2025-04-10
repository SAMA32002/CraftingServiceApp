using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Domain.DTOs
{
    public class PaymentDetailsDto
    {
        public int PaymentId { get; set; }
        public string ClientId { get; set; }
        public string CrafterId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
