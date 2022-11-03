using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Models.Product
{
    [Table("ProductCategoryproduct")]
    public class ProductCategoryproduct
    {

        public int ProductID { set; get; }

        public int CategoryID { set; get; }

        [ForeignKey("ProductID")]
        public ProducModel Product { set; get; }

        [ForeignKey("CategoryID")]
        public Categoryproduct Category { set; get; }
    }
}
