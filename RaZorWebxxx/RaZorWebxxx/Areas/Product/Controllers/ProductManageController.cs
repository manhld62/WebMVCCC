using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.AppUlti;

using RaZorWebxxx.Areas.Products.Models;
using RaZorWebxxx.Models;
using RaZorWebxxx.Models.Blog;
using RaZorWebxxx.Models.Product;
using XTL.Helpers;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;


namespace RaZorWebxxx.Areas.Products
{
    [Area("Product")]
    [Route("admin/productmanage/[action]/{id?}")]
    [Authorize(Roles = "admin")]
    public class ProductManageController : Controller
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;


        public ProductManageController(MyBlogContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { set; get; }

        // GET: Blog/Posts
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentpagee, int pagesize)
        {
            var post = _context.Products.Include(p => p.Author).OrderByDescending(p => p.DateUpdated);
            int totalpost = await post.CountAsync();
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
            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalpost;
            ViewBag.postIndex = (currentpagee - 1) * pagesize;

            var postsInPage = await post.Skip((currentpagee - 1) * pagesize).Take(pagesize).Include(p => p.ProductCategoryproducts)
                .ThenInclude(pc => pc.Category).ToListAsync();
            return View(postsInPage);
        }

        // GET: Blog/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Posts/Create
        public async Task<IActionResult> CreateAsync()
        {
            var categories = await _context.Categoryproducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        // POST: Blog/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel post)
        {
            var categories = await _context.Categoryproducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if (post.Slug == null)
            {
                post.Slug = Ulti.GenerateSlug(post.Title);
            }
            if (await _context.Products.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId = user.Id;
                _context.Add(post);
                if (post.CategoryIDs != null)
                {
                    foreach (var CateId in post.CategoryIDs)
                    {
                        _context.Add(new ProductCategoryproduct()
                        {
                            CategoryID = CateId,
                            Product = post
                        });

                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        // GET: Blog/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Products.Include(p => p.ProductCategoryproducts).FirstOrDefaultAsync(p => p.ProductId == id);
            if (post == null)
            {
                return NotFound();
            }


            var postEdit = new CreateProductModel()
            {
                ProductId = post.ProductId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIDs = post.ProductCategoryproducts.Select(pc => pc.CategoryID).ToArray(),
                Price = post.Price




            };
            var categories = await _context.Categoryproducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Blog/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel post)
        {
            if (id != post.ProductId)
            {
                return NotFound();
            }
            var categories = await _context.Categoryproducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if (post.Slug == null)
            {
                post.Slug = Ulti.GenerateSlug(post.Title);
            }
            if (await _context.Products.AnyAsync(p => p.Slug == post.Slug && p.ProductId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var postupdate = await _context.Products.Include(p => p.ProductCategoryproducts).FirstOrDefaultAsync(p => p.ProductId == id);
                    if (postupdate == null)
                    {
                        return NotFound();
                    }
                    postupdate.Title = post.Title;
                    postupdate.Description = post.Description;
                    postupdate.Content = post.Content;
                    postupdate.Published = post.Published;
                    postupdate.Slug = post.Slug;
                    postupdate.DateUpdated = DateTime.Now;
                    postupdate.Price = post.Price;
                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };
                    var oldcateid = postupdate.ProductCategoryproducts.Select(c => c.CategoryID).ToArray();
                    var newcateid = post.CategoryIDs;
                    var removeCatePost = from postcate in postupdate.ProductCategoryproducts
                                         where (!newcateid.Contains(postcate.CategoryID))
                                         select postcate;
                    _context.ProductCategoryproducts.RemoveRange(removeCatePost);
                    var addCateIds = from Cateid in newcateid
                                     where !oldcateid.Contains(Cateid)
                                     select Cateid;
                    foreach (var Cateid in addCateIds)
                    {
                        _context.ProductCategoryproducts.Add(new ProductCategoryproduct()
                        {
                            ProductID = id,
                            CategoryID = Cateid

                        });
                    }





                    _context.Update(postupdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa Câp Nhật Bài Viết";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Blog/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Products.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            _context.Products.Remove(post);
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa xóa bài viết : " + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        public class UploadOneFile
        {
            [Required(ErrorMessage = "Phải Chọn File Upload")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "png,jpg,jpeg,gif")]
            [Display(Name = "Chọn File Upload")]

            public IFormFile FileUpload { set; get; }
        }


        [HttpGet]
        public IActionResult UploadPhoto(int id)
        {
            var product = _context.Products.Where(e => e.ProductId == id).Include(p => p.Photos).FirstOrDefault();
            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }
            ViewData["product"] = product;
            return View(new UploadOneFile());
        }
        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadPhotoAsync(int id, [Bind("FileUpload")] UploadOneFile f)
        {
            var product = _context.Products.Where(e => e.ProductId == id).Include(p => p.Photos).FirstOrDefault();
            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }
            ViewData["product"] = product;
            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(f.FileUpload.FileName);
                var file = Path.Combine("Uploads", "Products", file1);
                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }
                _context.Add(new ProductPhoto()
                {
                    ProductID = product.ProductId,
                    FileName = file1
                });
                await _context.SaveChangesAsync();


            }

            return View(f);
        }
        [HttpPost]
        public IActionResult ListPhotos(int id)
        {
            var product = _context.Products.Where(e => e.ProductId == id).Include(p => p.Photos).FirstOrDefault();
            if (product == null)
            {
                return Json(
                    new
                    {
                        success = 0,
                        message = "Product Not Found",

                    }
                    );
            }
           var listphoto= product.Photos.Select(photo => new
            {
                id = photo.Id,
                path = "/contents/Products/" + photo.FileName
            });
            return Json(
                new
                {
                    success = 1,
                   
                    photos = listphoto
                    
                }
            );
            

        }
        public IActionResult DeletePhoto(int id)
        {
            var photo = _context.ProductPhotos.Where(p => p.Id == id).FirstOrDefault();
            if(photo != null)
            {
                _context.Remove(photo);
                _context.SaveChanges();
                var filename = "Uploads/Products/" + photo.FileName;
                System.IO.File.Delete(filename);

            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UploadPhotoApI(int id, [Bind("FileUpload")] UploadOneFile f)
        {
            var product = _context.Products.Where(e => e.ProductId == id).Include(p => p.Photos).FirstOrDefault();
            if (product == null)
            {
                return NotFound("Không có sản phẩm");
            }
          
            if (f != null)
            {
                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(f.FileUpload.FileName);
                var file = Path.Combine("Uploads", "Products", file1);
                using (var filestream = new FileStream(file, FileMode.Create))
                {
                    await f.FileUpload.CopyToAsync(filestream);
                }
                _context.Add(new ProductPhoto()
                {
                    ProductID = product.ProductId,
                    FileName = file1
                });
                await _context.SaveChangesAsync();


            }

            return Ok();
        }
    }
}
        
     
    

