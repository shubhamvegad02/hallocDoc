using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class UserAccessData
    {
        public int accountType { get; set; }

        public string? name { get; set; }

        public string? mobile { get; set; }

        public string? status { get; set; }

        public int? openReq { get; set; }

        public string? aspId { get; set; }
    }
}
