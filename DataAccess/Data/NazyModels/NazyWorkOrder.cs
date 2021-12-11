using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.NazyModels
{
    public class NazyWorkOrder
    {
        public int ID { get; set; }
        public string Brand { get; set; }
        public int OrderNum { get; set; }
        public string FabricType { get; set; }
        public string ArticleType { get; set; }
        public int ArticleNumber { get; set; }
        public string ArticleColor { get; set; }
        public int PieceCount { get; set; }
        public string PurchasesStr { get; set; }
        public string EmbroideryStr { get; set; }
        public string ServicesStr { get; set; }

        public static bool Validate(NazyWorkOrder input)
        {
            bool allowed = true;
            return allowed;
        }
    }
}
