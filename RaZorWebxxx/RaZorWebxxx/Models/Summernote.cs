using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Models
{
    public class Summernote
    {
        public Summernote(string iDEdittor, bool loadLibrary=true)
        {
            IDEdittor = iDEdittor;
            LoadLibrary = loadLibrary;
        }

        public string IDEdittor { set; get; }
        public bool LoadLibrary { set; get; }
        public int height { set; get; } = 120;
        public string toolbar { set; get; } = @"
            [
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'picture', 'video','elfinder']],
                ['height', ['height']]
                ['view', ['fullscreen', 'codeview', 'help']]
            ]";
    }
}
