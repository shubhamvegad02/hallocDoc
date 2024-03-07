using halloDocEntities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class SendOrder
    {
        public string profession { get; set; }

        public string business { get; set; }

        public int Contact { get; set; }

        public string email { get; set; }

        public int fax { get; set; }

        public string orderDetail { get; set; }

        public int refill { get; set; }

        public List<Healthprofessionaltype>  professionList { get; set; }

        public List<Healthprofessional> businessList { get; set; }
    }
}
