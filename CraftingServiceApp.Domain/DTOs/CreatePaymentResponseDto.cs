using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Domain.DTOs
{
    public class CreatePaymentResponseDto
    {
        public int PaymentId { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsSuccess { get; set; }
    }
}
