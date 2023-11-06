using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;

namespace OrderingFood.Models
{
    public partial class Bill
    {
        public Bill()
        {
            DetailsBills = new HashSet<DetailsBill>();
        }
        public Guid BillId { get; set; }
        [ DisplayName("Trạng thái")]
        public string? Status { get; set; }
        [DisplayName("Tổng tiền")]
        public Decimal? Total { get; set; }
        [DisplayName("Ngày lập")]
        public DateTime? CreatedDate { get; set; }
        [DisplayName("Khách hàng")]
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<DetailsBill> DetailsBills { get; set; }
    }
}
