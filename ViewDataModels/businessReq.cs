using System.ComponentModel.DataAnnotations;

namespace hallocDoc.ViewDataModels
{
    public class businessReq
    {
        [StringLength(100)]
        [Required(ErrorMessage = "First Name is Required")]
        public string? BFirstName { get; set; }


        [StringLength(100)]
        public string? BLastName { get; set; }

        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [StringLength(10)]
        [Required(ErrorMessage = "Mobile Number is Required")]
        public string? BMobile { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [Required(ErrorMessage = "Email is Required")]
        public string? BEmail { get; set; }

        [StringLength(50)]
        public string? BusinessName { get; set; }

        [StringLength(50)]
        public string? CaseNumber { get; set; }

        [StringLength(500)]
        [Required]
        public string? Notes { get; set; }

        [Required]
        public string FirstName { get; set; } = "";


        
        public string? LastName { get; set; }

        public DateTime CreatedDate { get; set; }

        [EmailAddress]
        [Required]
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

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }
}
