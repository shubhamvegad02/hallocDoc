using halloDocEntities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class AdminProfile
    {
        public string? username { get; set; }


        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain Alphanumeric and Special characters")]
        [DataType(DataType.Password)]
        public string? password { get; set; }

        public string? status { get; set; }

        public string? role { get; set; }

        public List<string>? statusList { get; set; }

        public List<string>? roleList { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string? firstname { get; set; }

        public string? lastname { get; set; }

        [EmailAddress]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,4})$", ErrorMessage = "Domain is not real..")]
        public string? email { get; set; }

        [Required]
        [Display(Name = "Confirm Email")]
        [Compare("email", ErrorMessage = "Confirm Email doesn't match, Type again !")]
        public string? confirmMail { get; set; }

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? mobile { get; set; }

        public string? address1 { get; set; }

        public string? address2 { get; set; }

        public string? city { get; set; }

        public List<string>? AdminStateList { get; set; }

        public List<string>? stateList { get; set; }

        public string? state { get; set; }

        public string? zipcode { get; set; }

        public string? billingMobile { get; set; }

        public List<Region>? regionList { get; set; }
    }
}
