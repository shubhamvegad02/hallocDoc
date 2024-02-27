using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class ARequestCount
    {
        public int newc { get; set; }
        public int pendingc { get; set; }
        public int activec { get; set; }
        public int concludec { get; set; }
        public int closec { get; set; }
        public int unpaidc { get; set; }
    }
}
