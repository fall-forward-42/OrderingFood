using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderingFood.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập !"), DisplayName("Tên danh mục")]
        public string? Name { get; set; }
        [ DisplayName("Hình ảnh")]
        public string? ImageUrl { get; set; }
        [DisplayName("Cung ứng")]
        public bool? IsActive { get; set; }
        [ DisplayName("Ngày tạo")]
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
