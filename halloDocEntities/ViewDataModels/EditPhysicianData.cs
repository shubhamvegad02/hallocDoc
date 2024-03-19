using Microsoft.Extensions.FileProviders;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocEntities.ViewDataModels
{
    public class EditPhysicianData
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
        public string? firstname { get; set; }

        public string? lastname { get; set; }

        [EmailAddress]
        [Required]
        public string? email { get; set; }

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? mobile { get; set; }

        public string? medicallicence { get; set; }

        public string? npinumber { get; set; }

        public string? semail { get; set; }

        public List<string>? ProviderStateList { get; set; }

        public List<string>? stateList { get; set; }

        public string? address1 { get; set; }

        public string? address2 { get; set; }

        public string? city { get; set; }

        public string? state { get; set; }

        public string? zipcode { get; set; }

        public string? billingMobile { get; set; }

        /*public List<Region>? regionList { get; set; }*/

        public string? businessname { get; set; }

        public string? businesssite { get; set; }

        public IFileInfo photo { get; set; }

        public IFileInfo sign { get; set; }

        public string? adminNotes { get; set; }

        public bool ICA { get; set; }

        public string? ICADoc { get; set; }

        public bool BC { get; set; }

        public string? BCDoc {get; set;}

        public bool HC { get; set; }

        public string? HCDoc {get; set;}

        public bool NDA { get; set; }

        public string? NDADoc {get; set;}

        public bool LD { get; set; }

        public string? LDDoc { get; set; }
    }
}
