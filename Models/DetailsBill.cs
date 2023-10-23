using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class DetailsBill
    {
        public Guid DetailsBillId { get; set; }
        public Guid? ProductId { get; set; }
        public int? Quantity { get; set; }
        public Guid? BillId { get; set; }

        public virtual Bill? Bill { get; set; }
        public virtual Product? Product { get; set; }
    }
}
