using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class Bill
    {
        public Bill()
        {
            DetailsBills = new HashSet<DetailsBill>();
        }

        public Guid BillId { get; set; }
        public bool? Status { get; set; }
        public decimal? Total { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<DetailsBill> DetailsBills { get; set; }
    }
}
