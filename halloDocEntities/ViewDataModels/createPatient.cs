/*using hallocDoc.Controllers;
*/using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class createPatient
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain Alphanumeric and Special characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string cpassword { get; set; }
    }
}
