using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class ViewUploadedDoc
    {
        public string patientName { get; set; }

        public string confirmation { get; set; }

        public DateTime uploadDate { get; set; }

        public string fileName { get; set; }

         
    }
}
