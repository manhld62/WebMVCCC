using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaZorWebxxx.Models.Contacts
{
    public class Contact
    {
        [Key]
        public int Id { set; get; }
        [Column(TypeName="nvarchar")]
        [StringLength(50)]

        [Required(ErrorMessage ="Phải Nhập {0}")]
        [Display(Name="Họ Tên")]
        public string FullName { set; get; }
        [Required(ErrorMessage = "Phải Nhập {0}")]
        [EmailAddress(ErrorMessage ="Phải là địa chỉ email")]


        [StringLength(100)]
        [Display(Name = "Địa Chỉ Email")]
        public string Email { set; get; }
        public DateTime DataSent { set; get; }
        [Required(ErrorMessage = "Phải Nhập {0}")]
        [Display(Name = "Tin Nhắn")]
        public string Message { set; get; }
        [StringLength(50)]
        [Phone(ErrorMessage ="Phải Là Số Điện Thoại")]
        [Display(Name = "Số Điên Thoại")]
        public string Phone { set; get; }
    }
    
}
