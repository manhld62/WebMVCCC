using RaZorWebxxx.Models.Blog;
using RaZorWebxxx.Models.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RaZorWebxxx.Models.Product;

namespace RaZorWebxxx.Areas.Products.Models
{
    public class CreateProductModel : ProducModel
    {
        [Display(Name="Chuyên Mục")]
        public int[] CategoryIDs { set; get; }

    }
   
    
}
