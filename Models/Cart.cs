using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class Cart
    {
        public Guid CartId { get; set; }
        public Guid? ProductId { get; set; }
        public int? Quantity { get; set; }
        public Guid? UserId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }
    }
}
