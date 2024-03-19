using halloDocEntities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class AProvider
    {
        public List<string>? regionList { get; set; }

        public string? name { get; set; }

        public string? role { get; set; }

        public string? onCall { get; set; }

        public string? status { get; set; }

        public string? communicationType { get; set; }

        public string? popupMsg { get; set; }

        public string? region { get; set; }

        public int physicianId { get; set; }

        public string email { get; set; }
    }
}
