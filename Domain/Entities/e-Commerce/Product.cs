using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.e_Commerce
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; }
        // Navigation property for the category this product belongs to
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        // Navigation property for order items that include this product
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public ICollection<Rate> ProductRates { get; set; } = new List<Rate>();



    }
}
