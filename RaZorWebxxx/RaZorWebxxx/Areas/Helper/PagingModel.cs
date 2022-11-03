using System;


namespace XTL.Helpers
{
    public class PagingModel {
        public int currentpage { set; get; }
        public int countpage { set; get; }
        public Func<int?, string> generateUrl { set; get; }
    
    
    }

}
