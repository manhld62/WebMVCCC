using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

 namespace RaZorWebxxx.Models.Product
{
    [Table("productPhoto")]
    public class ProductPhoto
    {
        [Key]
        public int Id { set; get; }
        public string FileName { set; get; }
        public int ProductID { set; get; }
        [ForeignKey("ProductID")]
        public ProducModel Product { set; get; }
    }
}
