using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RaZorWebxxx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaZorWebxxx.Models.Product;
using XTL.Helpers;
using RaZorWebxxx.Areas.Product.Service;
using RaZorWebxxx.Areas.Product.Models;

namespace RaZorWebxxx.Areas.Products.Controllers


{
    [Area("Product")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;
        private readonly MyBlogContext _context;
        public readonly CardService _cardService;
      public ViewProductController(ILogger<ViewProductController> logger,MyBlogContext context,CardService cardService)
        {
            _logger = logger;
            _context = context;
            _cardService = cardService;
        }
        [Route("/product/{categoryslug?}")]
        public IActionResult Index(string categoryslug,[FromQuery(Name ="p")]int currentpagee, int pagesize)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            Categoryproduct category = null;
            if (!string.IsNullOrEmpty(categoryslug)) {
                category= _context.Categoryproducts.Where(c => c.Slug == categoryslug).Include(c => c.CategoryChildren).FirstOrDefault();
                if (category == null)
                {
                    return NotFound("Không Tìm thấy danh mục cần tìm");
                }
            }
            var posts = _context.Products.Include(p => p.Author).Include(p => p.Photos).Include(p=>p.ProductCategoryproducts).ThenInclude(p => p.Category).AsQueryable();
           posts= posts.OrderByDescending(p => p.DateUpdated);
            if(category != null)
            {
                var ids = new List<int>();
                category.ChildCategoryIds( ids,null);
                ids.Add(category.Id);
                posts=   posts.Where(p => p.ProductCategoryproducts.Where(pc => ids.Contains(pc.CategoryID)).Any());
            }
            int totalpost = posts.Count();

            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalpost / pagesize);
            if (currentpagee > countPages) currentpagee = countPages;
            if (currentpagee < 1) currentpagee = 1;
            var pagingModel = new PagingModel()
            {
                countpage = countPages,
                currentpage = currentpagee,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })

            };
            var postsInPage = posts.Skip((currentpagee - 1) * pagesize).Take(pagesize);
             
            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalpost;







            ViewBag.category = category;


            return View(postsInPage.ToList());
        }
        [Route("/product/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            var post = _context.Products.Where(p => p.Slug == postslug).Include(p => p.Author).Include(p=>p.Photos).Include(p => p.ProductCategoryproducts).ThenInclude(pc => pc.Category).FirstOrDefault();
            if (post == null){
                return NotFound("Không Tìm Thấy Bài Viết");
                    
                    }
            Categoryproduct category = post.ProductCategoryproducts.FirstOrDefault()?.Category;
            ViewBag.category = category;
            return View(post);

        }
        private List<Categoryproduct> GetCategories()
        {
            var categories = _context.Categoryproducts.Include(c => c.CategoryChildren).AsEnumerable().Where(c => c.ParentCategory == null).ToList();
            return categories;

        }
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _context.Products
                .Where(p => p.ProductId == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = _cardService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào Session
            _cardService.SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }
        

// Hiện thị giỏ hàng
          [Route("/cart", Name = "cart")]
     public IActionResult Cart()
        {
            return View(_cardService.GetCartItems());
        }
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = _cardService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            _cardService.SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = _cardService.GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
           _cardService.SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }
        [Route("/checkout")]
        public IActionResult Checkout()
        {






            _cardService.ClearCart();
            return Content("Da gui don hang");

        }

    }
}
