using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OrderingFood.Models
{
    public partial class DetailsBill
    {
        public Guid DetailsBillId { get; set; }
        [DisplayName("Món ăn")]
        public Guid? ProductId { get; set; }
        [DisplayName("Số lượng")]
        public int? Quantity { get; set; }
        [DisplayName("Hóa đơn")]
        public Guid? BillId { get; set; }
        public virtual Bill? Bill { get; set; }
        public virtual Product? Product { get; set; }
    }
}
