using halloDocEntities.DataModels;
using System.ComponentModel.DataAnnotations;

namespace hallocDoc.ViewDataModels
{
    public class conciergeReq
    {

        [Required(ErrorMessage = "First Name is Required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? CFN { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? CLN { get; set; }

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? CMobile { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [Required(ErrorMessage = "Email is Required")]
        public string? CMail { get; set; }

        [Required]
        [StringLength(100)]
        public string? property { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? Street { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? City { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? State { get; set; }

        public string? ZipCode { get; set; }

        [Required]
        public string? Notes { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; } = "";



        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [Required]
        public string Email { get; set; } = null!;

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? Mobile { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
