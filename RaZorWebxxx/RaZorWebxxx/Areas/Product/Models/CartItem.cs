using RaZorWebxxx.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RaZorWebxxx.Areas.Product.Models
{
    public class CartItem
    {
        public int quantity { set; get; }
        public ProducModel product { set; get; }
    }
}
