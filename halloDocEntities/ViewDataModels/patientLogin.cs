using halloDocEntities.DataModels;
using System.ComponentModel.DataAnnotations;

namespace halloDocEntities.ViewDataModels
{
    public class patientLogin
    {
        [EmailAddress]
        /*[RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]{1,4})$", ErrorMessage = "Domain is not real..")]*/
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? Aspid { get; set; }

        public string? Status { get; set; }
    }
}
