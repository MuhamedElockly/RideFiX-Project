using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData.Enums;

namespace SharedData.DTOs.E_CommerceDTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public OrderStatus OrderState { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
