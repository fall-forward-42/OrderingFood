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
            Carts = new HashSet<Cart>();
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
        
        [DisplayName("Nhân viên tạo đơn")]
        public Guid? EmployeeId { get; set; }

        public virtual User? User { get; set; }
        public virtual User? Employee { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }


        public string PriceToVND
        {
            get => string.Format("{0:C}", Total) + " VNĐ";
        }
    }
}
