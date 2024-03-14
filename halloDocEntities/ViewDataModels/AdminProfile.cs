using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class AdminProfile
    {
        public string? username { get; set; }

        public string? password { get; set; }

        public string? status { get; set; }

        public string? role { get; set; }

        public List<string> statusList { get; set; }

        public List<string> roleList { get; set; }

        public string? firstname { get; set; }

        public string? lastname { get; set; }

        public string? email { get; set; }

        public string? confirmMail { get; set; }

        public string? mobile { get; set; }

        public string? address1 { get; set; }

        public string? address2 { get; set; }

        public string? city { get; set; }

        public string? state { get; set; }

        public List<string> stateList { get; set; }

        public string? zipcode { get; set; }

        public string? billingMobile { get; set; }
    }
}
