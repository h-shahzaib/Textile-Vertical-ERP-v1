using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class NazyOrder
    {
        public int ID { get; set; }
        public string Brand { get; set; }
        public string OrderNo { get; set; }
        public string MainFabric { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleType { get; set; }
        public string MainImage { get; set; }
        public string ColorDetailStr { get; set; }
        public string Status { get; set; }
    }
}
