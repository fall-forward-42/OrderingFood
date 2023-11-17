using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OrderingFood.Models
{
    public partial class Contact
    {
        public Guid ContactId { get; set; }

        [DisplayName("Tên người gửi")]
        public string? Name { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Chủ đề")]
        public string? Subject { get; set; }

        [DisplayName("Nội dung")]
        public string? Message { get; set; }

        [DisplayName("Ngày gửi")]
        public DateTime? CreatedDate { get; set; }
    }
}
