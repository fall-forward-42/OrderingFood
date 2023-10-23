using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class Payment
    {
        public Guid PaymentId { get; set; }
        public string? Name { get; set; }
        public string? CardNo { get; set; }
        public string? ExpiryDate { get; set; }
        public int? CvvNo { get; set; }
        public string? Address { get; set; }
        public string? PaymentMode { get; set; }
        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
