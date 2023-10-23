using System;
using System.Collections.Generic;

namespace OrderingFood.Models
{
    public partial class Contact
    {
        public Guid ContactId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
