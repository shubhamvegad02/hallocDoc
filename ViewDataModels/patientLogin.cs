using hallocDoc.DataModels;
using System.ComponentModel.DataAnnotations;

namespace hallocDoc.ViewDataModels
{
    public class patientLogin
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
