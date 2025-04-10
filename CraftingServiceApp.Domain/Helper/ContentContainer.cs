using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Domain.Helper
{
    public class ContentContainer<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }

        public ContentContainer(T data, string message = null)
        {
            Data = data;
            Message = message ?? "Operation successful";
        }
    }
}
