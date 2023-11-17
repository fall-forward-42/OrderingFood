using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [DisplayName("Hình ảnh")]
        public string? ImageUrl { get; set; }
        [DisplayName("Ngày lập")]
        public DateTime? CreatedDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email !"), DisplayName("Email")]
        [EmailAddress(ErrorMessage ="Email không hợp lệ !")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu !"), DisplayName("Mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu không đủ 6 ký tự !")]
        public string? Password { get; set; }
        //Show encode password
        public string EncodedPassword
        {
            get => EncodePassword(Password!);
        }



        [NotMapped]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu !"), DisplayName("Mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu không đủ 6 ký tự !")]
        public string? NewPassword { get; set; }


        [NotMapped]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu !"), DisplayName("Mật khẩu xác nhận")]
        [MinLength(6, ErrorMessage = "Mật khẩu không đủ 6 ký tự !")]
       [Compare("NewPassword", ErrorMessage = "Mật khẩu không trùng khớp !")]
        public string? ConfPassword { get; set; }

        [DisplayName("Loại người dùng")]
        public string? TypeAccount { get; set; }

        public  string EncodePassword(string pass)
        {
           
                byte[] enc = new byte[Password!.Length];
                enc = System.Text.Encoding.UTF8.GetBytes(pass);
                string encData = Convert.ToBase64String(enc);
                return encData;
        }


        public string DecodePassword(string Encpass) 
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(Encpass);
            int charCount = utf8Decode.GetCharCount(todecode_byte,0,todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte,0,todecode_byte.Length,decoded_char, 0);
            string r = new string(decoded_char);
            return r;

        }


        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

        
    }
}
