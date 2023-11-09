using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OrderingFood.Models
{
    public partial class Cart
    {
        public Guid CartId { get; set; }
        [DisplayName("Món ăn")]
        public Guid? ProductId { get; set; }
        [DisplayName("Số lượng")]
        public int? Quantity { get; set; }
        [DisplayName("Khách hàng")]
        public Guid? UserId { get; set; }
        [DisplayName("Trạng thái")]
        public string? Status { get; set; }
        [DisplayName("Hóa đơn")]
        public Guid? BillId { get; set; }
        public virtual Bill? Bill { get; set; }
        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }

        

    }
}
