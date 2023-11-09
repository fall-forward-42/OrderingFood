using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OrderingFood.Models
{
    public partial class Payment
    {
        public Guid PaymentId { get; set; }
        [DisplayName("Tên đại diện")]
        public string? Name { get; set; }
        [DisplayName("Số tài khoản")]

        public string? CardNo { get; set; }
        [DisplayName("Ngày hết hạn")]

        public string? ExpiryDate { get; set; }
        public int? CvvNo { get; set; }
        public string? Address { get; set; }
        public string? PaymentMode { get; set; }
        [DisplayName("Khách hàng")]

        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
