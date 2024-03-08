using halloDocEntities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class SendOrder
    {
        public string profession { get; set; }

        public string business { get; set; }

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string Contact { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [Required(ErrorMessage = "Email is Required")]
        [StringLength(50)]
        public string email { get; set; }

        [Phone]
        [RegularExpression(@"^\+?[0-9. \-]+$", ErrorMessage = "Invalid fax number format")]
        public string fax { get; set; }

        public string orderDetail { get; set; }

        public int refill { get; set; }

        public List<Healthprofessionaltype>  professionList { get; set; }

        public List<Healthprofessional> businessList { get; set; }
    }
}
