using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Security.Policy;

namespace OrderingFood.Models
{
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            DetailsBills = new HashSet<DetailsBill>();
        }

        public Guid ProductId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên món !"), DisplayName("Tên món ăn")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả món !"), DisplayName("Mô tả món ăn")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá tiền !"), DisplayName("Giá món ăn")]
        public Decimal? Price { get; set; }
    
        [Required(ErrorMessage = "Vui lòng nhập số lượng !"), DisplayName("Số lượng")]
        public int? Quantity { get; set; }
        [DisplayName("Hình ảnh món ăn")]
        public string? ImageUrl { get; set; }
        [DisplayName("Loại món")]
        public Guid? CategoryId { get; set; }
        [DisplayName("Cung ứng")]
        public string? IsActive { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreatedDate { get; set; }
        public virtual Category? Categories { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<DetailsBill> DetailsBills { get; set; }

        public string PriceToVND
        {
            get => string.Format("{0:C}", Price) +" VNĐ";
        }

    }
}
