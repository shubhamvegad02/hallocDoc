using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class businessReq
    {
        [StringLength(100)]
        [Required(ErrorMessage = "First Name is Required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? BFirstName { get; set; }


        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? BLastName { get; set; }

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? BMobile { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,3})$", ErrorMessage = "Domain is not real..")]
        public string? BEmail { get; set; }

        [StringLength(50)]

        public string? BusinessName { get; set; }

        [StringLength(50)]
        public string? CaseNumber { get; set; }

        [StringLength(500)]
        [Required]
        public string? Notes { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; } = "";


        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,3})$", ErrorMessage = "Domain is not real..")]
        public string Email { get; set; } = null!;

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? Mobile { get; set; }
        /*
                [Column(TypeName = "character varying")]
                [Required]
                public string? PasswordHash { get; set; }*/
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? Street { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? City { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string? State { get; set; }

        public string? ZipCode { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
