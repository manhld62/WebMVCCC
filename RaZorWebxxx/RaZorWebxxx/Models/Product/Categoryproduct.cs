using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaZorWebxxx.Models.Product
{
    [Table("Categoryproduct")]
    public class Categoryproduct
    {

        [Key]
        public int Id { get; set; }

        // Category cha (FKey)
        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }

        // Tiều đề Category
        [Required(ErrorMessage = "Phải có tên danh mục")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Tên danh mục")]
        public string Title { get; set; }

        // Nội dung, thông tin chi tiết về Category
        [DataType(DataType.Text)]
        [Display(Name = "Nội dung danh mục")]
        public string Description { set; get; }

        //chuỗi Url
        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]
        public string Slug { set; get; }

        // Các Category con
        public ICollection<Categoryproduct> CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]


        public Categoryproduct ParentCategory { set; get; }
        public void ChildCategoryIds(List<int> list, ICollection<Categoryproduct> childcates = null)
        {
            if (childcates == null)
                childcates = this.CategoryChildren;
            foreach (Categoryproduct category in childcates)
            {
                list.Add(category.Id);
                ChildCategoryIds(list, category.CategoryChildren);
            }
        }
        public List<Categoryproduct> ListParents()
        {
            List<Categoryproduct> li = new List<Categoryproduct>();
            var parent = this.ParentCategory;
            while (parent != null)
            {
                li.Add(parent);
                parent = parent.ParentCategory;
            }
            li.Reverse();
            return li;
        }
    }
}