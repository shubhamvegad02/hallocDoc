using halloDocEntities.DataModels;
using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class ADashTable
    {
        public int? rid { get; set; }
        public string? guid { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string? name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,3})$", ErrorMessage = "Domain is not real..")]
        public string? email { get; set; }

        public DateTime dob { get; set; }

        public string? requstor { get; set; }

        public DateTime reqDate { get; set; }


        [Required(ErrorMessage = "Mobile Number is Required")]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        public string? mobile { get; set; }

        public string? address { get; set; }

        public string? notes { get; set; }

        public string? provider { get; set; }

        public string? relation { get; set; }

        public string? region { get; set; }

        public int? regionId { get; set; }

        public int? phyId { get; set; }

        public int? status { get; set; }

        public int? caseTag { get; set; }

        public string? PhysicianName { get; set; }


    }
}
