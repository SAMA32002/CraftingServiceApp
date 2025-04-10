using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Domain.DTOs
{
    public class CreatePaymentRequestDto
    {
        public string ClientId { get; set; }
        public string CrafterId { get; set; }
        public int RequestId { get; set; }
        public int ServiceId { get; set; }
        public decimal Amount { get; set; }
    }
}
