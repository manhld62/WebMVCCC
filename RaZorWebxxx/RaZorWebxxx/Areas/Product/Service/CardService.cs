using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RaZorWebxxx.Areas.Product.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Areas.Product.Service
{
    public class CardService
    {
        public const string CARTKEY = "cart";
        private readonly IHttpContextAccessor _context;
        private readonly HttpContext HttpContext;
        public CardService(IHttpContextAccessor context)
        {
            _context = context;
            HttpContext = context.HttpContext;
        }
        public List<CartItem> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
       public  void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
       public void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
    }
}
