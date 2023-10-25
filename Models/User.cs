using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderingFood.Models
{
    public partial class User 
    {
       
        public User()
        {
            Bills = new HashSet<Bill>();
            Carts = new HashSet<Cart>();
            Payments = new HashSet<Payment>();
        }
        
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên !"), DisplayName("Họ tên")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại !"), DisplayName("Số điện thoại")]
        [MaxLength(10,ErrorMessage ="Số điện thoại không hợp lệ !")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ !"), DisplayName("Địa chỉ")]
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email !"), DisplayName("Email")]
        [EmailAddress(ErrorMessage ="Email không hợp lệ !")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu !"), DisplayName("Mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu không đủ 6 ký tự !")]
        public string? Password { get; set; }
        public string? TypeAccount { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

        
    }
}
