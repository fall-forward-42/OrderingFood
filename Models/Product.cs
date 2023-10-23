using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            DetailsBills = new HashSet<DetailsBill>();
        }

        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<DetailsBill> DetailsBills { get; set; }
    }
}
