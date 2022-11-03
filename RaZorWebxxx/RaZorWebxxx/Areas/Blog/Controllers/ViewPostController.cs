using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RaZorWebxxx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTL.Helpers;

namespace RaZorWebxxx.Areas.Blog
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> _logger;
        private readonly MyBlogContext _context;
      public ViewPostController(ILogger<ViewPostController> logger,MyBlogContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Route("/post/{categoryslug?}")]
        public IActionResult Index(string categoryslug,[FromQuery(Name ="p")]int currentpagee, int pagesize)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;
            Category category = null;
            if (!string.IsNullOrEmpty(categoryslug)) {
                category= _context.Categories.Where(c => c.Slug == categoryslug).Include(c => c.CategoryChildren).FirstOrDefault();
                if (category == null)
                {
                    return NotFound("Không Tìm thấy danh mục cần tìm");
                }
            }
            var posts = _context.Posts.Include(p => p.Author).Include(p => p.PostCategories).ThenInclude(p => p.Category).AsQueryable();
            posts.OrderByDescending(p => p.DateUpdated);
            if(category != null)
            {
                var ids = new List<int>();
                category.ChildCategoryIds( ids,null);
                ids.Add(category.Id);
                posts=   posts.Where(p => p.PostCategories.Where(pc => ids.Contains(pc.CategoryID)).Any());
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
        [Route("/post/{postslug}.html")]
        public IActionResult Detail(string postslug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            var post = _context.Posts.Where(p => p.Slug == postslug).Include(p => p.Author).Include(p => p.PostCategories).ThenInclude(pc => pc.Category).FirstOrDefault();
            if (post == null){
                return NotFound("Không Tìm Thấy Bài Viết");
                    
                    }
            Category category = post.PostCategories.FirstOrDefault()?.Category;
            ViewBag.category = category;
            return View(post);

        }
        private List<Category> GetCategories()
        {
            var categories = _context.Categories.Include(c => c.CategoryChildren).AsEnumerable().Where(c => c.ParentCategory == null).ToList();
            return categories;

        }

    }
}
