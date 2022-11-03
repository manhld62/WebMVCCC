using RaZorWebxxx.Models.Blog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name="Chuyên Mục")]
        public int[] CategoryIDs { set; get; }

    }
   
    
}
