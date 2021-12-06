using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalLib.Data.EmbModels
{
    public class EMBTask
    {
        public int ID { get; set; }
        public string RefType { get; set; }
        public string RefKey { get; set; }
        public string Todo { get; set; }
        public bool Complete { get; set; }

        public static bool Validate(EMBTask task)
        {
            bool allowed = true;

            if (string.IsNullOrWhiteSpace(task.RefType)
                || string.IsNullOrWhiteSpace(task.RefKey)
                || string.IsNullOrWhiteSpace(task.Todo))
                allowed = false;

            return allowed;
        }
    }
}
