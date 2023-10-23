using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public Guid CategoryId { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
